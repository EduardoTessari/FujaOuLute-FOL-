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
    public TMP_Text goalList; // Arraste seu Text aqui para mostrar a lista de objetivos
 
    public Button confirmButton;
    public Button cancelButton;

    [Header("Audios")]
    [SerializeField] private AudioSource _dockAudioSource;
    [SerializeField] private AudioClip _successAudioClip;
    [SerializeField] private AudioClip _errorAudioClip;
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
        _dockAudioSource = GetComponent<AudioSource>();
        
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

        // 3. A MÁGICA: Popula o Dropdown com o inventário atual
        PopulateDropdown();
        UpdateProgressDisplay();
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

    // Esta função vai ler a receita e o progresso e escrever no Text
    private void UpdateProgressDisplay()
    {
        // 1. Pega a "lista de compras" (a receita) do Cérebro
        List<ItemRequirement> recipe = _boatProgress.recipe;

        // 2. Pega o "carrinho" (o que já coletamos)
        Dictionary<ItemList, int> collected = _boatProgress.GetCollectedItems();

        // 3. Constrói o texto (como você sugeriu)
        string progressString = "META DO BARCO:\n"; // \n = quebra de linha

        // 4. Loop "Para Cada" item na receita...
        foreach (ItemRequirement req in recipe)
        {
            int currentAmount = 0; // Começa em 0

            // 5. Checa se já temos algo desse item no "carrinho"
            if (collected.ContainsKey(req.item))
            {
                currentAmount = collected[req.item]; // Pega a quantidade
            }

            // 6. A "Interpolação de String" que você mencionou!
            progressString += $"{req.item.ToString()}: {currentAmount} / {req.amountNeeded}\n";
        }

        // 7. Coloca o texto final na UI
        goalList.text = progressString;
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
            _dockAudioSource.PlayOneShot(_errorAudioClip);
            return;
        }

        // Precisamos checar se o número é POSITIVO ANTES de fazer qualquer coisa.
        if (amountToDeposit <= 0)
        {
            Debug.Log("Quantidade inválida. Deve ser um número maior que zero.");
            _dockAudioSource.PlayOneShot(_errorAudioClip);
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
            _dockAudioSource.PlayOneShot(_successAudioClip);

            PopulateDropdown(); // Atualiza o Dropdown (ex: "Wood (15)")
            UpdateProgressDisplay(); // Atualiza a Meta (ex: "Madeira: 15 / 50")
        }
        else
        {
            // O RemoveItem falhou (jogador tentou depositar mais do que tinha)
            Debug.Log("Falha no depósito. Quantidade insuficiente.");
            _dockAudioSource.PlayOneShot(_errorAudioClip);
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

        // Limpa as referências
        _playerInventory = null;
        _boatProgress = null;

        // Limpa o campo de input para a próxima vez
        amountInput.text = "";

        // Esconde a janela
        gameObject.SetActive(false);
    }
}

