using UnityEngine;
using UnityEngine.UI; // Para Slider, Button
using TMPro; // Para Dropdown e InputField (assumindo que você usa TextMeshPro)
using System.Collections.Generic; // Para o Dicionário!
using System.Linq; // Para facilitar a leitura do Dicionário

public class DockUI : MonoBehaviour
{
    [Header("Conexões da UI")]
    public TMP_Dropdown itemDropdown; // Arraste seu Dropdown aqui
    public TMP_InputField amountInput; // Arraste seu InputField aqui
    public Button confirmButton;
    public Button cancelButton;

    // "Memória" - Para saber com quem estamos falando
    private Inventory _playerInventory;
    private BoatProgress _boatProgress;

    // Guarda os itens que estão no dropdown para consulta
    private List<ItemList> _itemsInDropdown = new List<ItemList>();

    /**
     * Prepara a janela antes de abri-la
     */
    private void Start()
    {
        // Conecta as funções aos botões
        confirmButton.onClick.AddListener(OnConfirm);
        cancelButton.onClick.AddListener(OnCancel);

        // Configura o InputField para aceitar apenas números (segunda camada de proteção)
        amountInput.contentType = TMP_InputField.ContentType.IntegerNumber;
    }

    /**
     * Esta é a função "MÁGICA" que a Doca vai chamar para abrir a janela
     */
    public void OpenWindow(Inventory playerInventory, BoatProgress boatProgress)
    {
        // 1. Guarda as referências
        _playerInventory = playerInventory;
        _boatProgress = boatProgress;

        // 2. Mostra a janela e pausa o jogo
        gameObject.SetActive(true);
        Time.timeScale = 0f; // Pausa o jogo

        // 3. A MÁGICA: Popula o Dropdown com o inventário atual
        PopulateDropdown();
    }

    /**
     * Esta é a função que responde sua curiosidade!
     */
    private void PopulateDropdown()
    {
        // 1. Limpa o dropdown e nossa lista de memória
        itemDropdown.ClearOptions();
        _itemsInDropdown.Clear();

        // 2. Pega o inventário ATUAL do jogador
        Dictionary<ItemList, int> inventory = _playerInventory.GetCurrentInventory();

        // 3. Cria a lista de "opções" para o dropdown
        List<string> options = new List<string>();

        // 4. Loop Mágico: Passa por cada item no inventário
        foreach (KeyValuePair<ItemList, int> itemPair in inventory)
        {
            // Pega o nome do item (ex: "Wood") e a quantidade (ex: 20)
            string optionText = $"{itemPair.Key.ToString()} ({itemPair.Value})";

            options.Add(optionText); // Adiciona "Wood (20)" na lista
            _itemsInDropdown.Add(itemPair.Key); // Guarda o "ItemType.Wood" na memória
        }

        // 5. Alimenta o dropdown com as opções que encontramos
        itemDropdown.AddOptions(options);
    }

    /**
     * Chamado quando o botão "Confirmar" é clicado
     */
    private void OnConfirm()
    {
        // --- 1. QUAL ITEM FOI ESCOLHIDO? ---
        // Pega o índice do item (ex: 0, 1, 2...) que está selecionado no dropdown
        int selectedIndex = itemDropdown.value;
        // Usa esse índice para pegar o ItemType correspondente da nossa lista de memória
        ItemList selectedItem = _itemsInDropdown[selectedIndex];

        // --- 2. QUAL QUANTIDADE FOI DIGITADA? ---
        int amountToDeposit;
        // Tenta converter o texto do input para um número.
        // Se falhar (ex: texto vazio), a Checagem 1 falha.
        if (!int.TryParse(amountInput.text, out amountToDeposit))
        {
            Debug.Log("Quantidade inválida.");
            // Futuramente: Tocar som de erro
            return;
        }

        // Precisamos checar se o número é POSITIVO ANTES de fazer qualquer coisa.
        if (amountToDeposit <= 0)
        {
            Debug.Log("Quantidade inválida. Deve ser um número maior que zero.");
            // Tocar som de erro
            return; // Para a execução aqui
        }

        // --- 3. A CHECAGEM DE LÓGICA ---
        // Checagem 2 (Positivo?) e 3 (O jogador TEM isso?)
        // Nós já fizemos a função RemoveItem ser inteligente, então só precisamos chamá-la!
        bool success = _playerInventory.RemoveItem(selectedItem, amountToDeposit);

        if (success)
        {
            // Deu certo! Agora podemos entregar ao barco
            _boatProgress.AddItemToBoat(selectedItem, amountToDeposit);
            // Futuramente: Tocar o "plin" de sucesso aqui
            CloseWindow();
        }
        else
        {
            // O RemoveItem falhou (jogador tentou depositar mais do que tinha)
            Debug.Log("Falha no depósito. Quantidade insuficiente.");
            // Futuramente: Tocar som de erro
        }
    }

    /**
     * Chamado quando o botão "Cancelar" é clicado
     */
    private void OnCancel()
    {
        CloseWindow();
    }

    /**
     * Uma função privada para limpar tudo
     */
    private void CloseWindow()
    {
        // Despausa o jogo
        Time.timeScale = 1f;

        // Limpa as referências
        _playerInventory = null;
        _boatProgress = null;

        // Limpa o campo de input para a próxima vez
        amountInput.text = "";

        // Esconde a janela
        gameObject.SetActive(false);
    }
}

