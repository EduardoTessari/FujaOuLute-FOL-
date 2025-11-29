using System.Collections.Generic; // Para o Dicionário!
using TMPro; // Para Dropdown e InputField (assumindo que você usa TextMeshPro)
using UnityEngine;
using UnityEngine.UI; // Para Slider, Button

public class DockUI : MonoBehaviour
{
    [Header("Conexões da UI")]
    public TMP_Dropdown itemDropdown; // Arraste seu Dropdown aqui
    public TMP_InputField amountInput; // Arraste seu InputField aqui
    public TMP_Text goalList; // Arraste seu Text aqui para mostrar a lista de objetivos

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

        // 3. A MÁGICA: Popula o Dropdown com o inventário atual
        PopulateDropdown();
        UpdateProgressDisplay();
    }

    /**
     * Esta é a função que responde sua curiosidade!
     */
    private void PopulateDropdown()
    {
        // 1. Limpa tudo da última vez que foi aberto (como antes)
        itemDropdown.ClearOptions();
        _itemsInDropdown.Clear();

        // --- A CORREÇÃO ESTÁ AQUI ---

        // 2. Pega a "lista de compras" (A RECEITA) do Cérebro
        List<ItemRequirement> recipe = _boatProgress.recipe;

        // 3. Cria a lista de opções de TEXTO para a UI
        List<string> options = new List<string>();

        // 4. O LOOP MÁGICO (AGORA LENDO A RECEITA)
        // Em vez de ler o inventário, vamos ler a RECEITA
        foreach (ItemRequirement req in recipe)
        {
            // 5. CHECAGEM: O jogador TEM este item da receita no inventário?
            // (Usamos a função que já existe no Inventory.cs!)
            int amountPlayerHas = _playerInventory.GetItemCount(req.item);

            // 6. O FILTRO!
            // Só adicionamos ao dropdown se o jogador tiver pelo menos 1
            if (amountPlayerHas > 0)
            {
                // Cria o texto: "Wood (20)"
                string optionText = $"{req.item.ToString()} ({amountPlayerHas})";
                options.Add(optionText);

                // E guardamos na nossa "lista secreta" para o OnConfirm
                _itemsInDropdown.Add(req.item);
            }
        }

        // --- FIM DA CORREÇÃO ---

        // 7. ALIMENTA O DROPDOWN (agora 100% filtrado)
        itemDropdown.AddOptions(options);

        // 8. LÓGICA DE SEGURANÇA (Se o jogador não tiver NENHUM item da receita)
        if (options.Count == 0)
        {
            // Mostra uma mensagem de "vazio"
            options.Add("Você não tem os itens da receita...");
            itemDropdown.ClearOptions(); // Limpa de novo (só por garantia)
            itemDropdown.AddOptions(options);

            // Desativa a interação para o jogador não tentar depositar "nada"
            itemDropdown.interactable = false;
            amountInput.interactable = false;
            confirmButton.interactable = false;
        }
        else
        {
            // Se temos itens, garante que tudo está interativo
            itemDropdown.interactable = true;
            amountInput.interactable = true;
            confirmButton.interactable = true;
        }
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
        // 1. Pega o Item
        int selectedIndex = itemDropdown.value;
        ItemList selectedItem = _itemsInDropdown[selectedIndex];

        // 2. Pega a Quantidade que o Jogador QUER dar
        int amountPlayerWantsToGive; // (Mudei o nome para ficar super claro)
        if (!int.TryParse(amountInput.text, out amountPlayerWantsToGive) || amountPlayerWantsToGive <= 0)
        {
            Debug.Log("Quantidade inválida. Deve ser um número maior que zero.");
            AudioManager.instance.PlayDepositSound(false);
            return;
        }

        // 3. Pergunta ao Cérebro quanto ele PRECISA
        int amountBoatStillNeeds = _boatProgress.GetAmountNeeded(selectedItem);
        if (amountBoatStillNeeds == 0)
        {
            Debug.Log("O barco não precisa mais deste item!");
            AudioManager.instance.PlayDepositSound(false);
            return;
        }

        // 4. A LÓGICA DO "MAGIC CAMPUS" (A Correção)
        // Começamos com o que o jogador quer dar
        int amountToActuallyDeposit = amountPlayerWantsToGive;

        // E limitamos ao que o barco precisa
        if (amountToActuallyDeposit > amountBoatStillNeeds)
        {
            amountToActuallyDeposit = amountBoatStillNeeds; // A sua lógica!

            // "Escreve de volta" no InputField para o jogador VER a correção
            amountInput.text = amountToActuallyDeposit.ToString();
        }

        // 5. A TRANSAÇÃO (USANDO A VARIÁVEL CORRETA!)
        // Agora tentamos remover a quantidade CORRIGIDA
        bool success = _playerInventory.RemoveItem(selectedItem, amountToActuallyDeposit);

        if (success)
        {
            // E entregamos a quantidade CORRIGIDA
            _boatProgress.AddItemToBoat(selectedItem, amountToActuallyDeposit);
            AudioManager.instance.PlayDepositSound(true);

            PopulateDropdown();
            UpdateProgressDisplay();
        }
        else
        {
            // Isso agora só vai falhar se o jogador tentou dar 5, mas só tinha 3.
            Debug.Log("Falha no depósito. Quantidade insuficiente.");
            AudioManager.instance.PlayDepositSound(false);
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

