using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DockUI : MonoBehaviour
{
    [Header("Conexões da UI")]
    public TMP_Dropdown itemDropdown;
    public TMP_InputField amountInput;
    public TMP_Text goalList;
    public Button confirmButton;
    public Button cancelButton;

    private InventoryUI _playerInventory;
    private BoatProgress _boatProgress;
    private List<ItemData> _itemsInDropdown = new List<ItemData>();

    private void Start()
    {
        confirmButton.onClick.AddListener(OnConfirm);
        cancelButton.onClick.AddListener(OnCancel);
        amountInput.contentType = TMP_InputField.ContentType.IntegerNumber;
    }

    public void OpenWindow(InventoryUI playerInventory, BoatProgress boatProgress)
    {
        _playerInventory = playerInventory;
        _boatProgress = boatProgress;

        gameObject.SetActive(true);
        PopulateDropdown();
        UpdateProgressDisplay();
    }

    private void PopulateDropdown()
    {
        itemDropdown.ClearOptions();
        _itemsInDropdown.Clear();

        List<ItemRequirement> recipe = _boatProgress.recipe;
        List<string> options = new List<string>();

        foreach (ItemRequirement req in recipe)
        {
            int amountPlayerHas = _playerInventory.GetItemCount(req.item);

            if (amountPlayerHas > 0)
            {
                string optionText = $"{req.item.itemName} ({amountPlayerHas})";
                options.Add(optionText);
                _itemsInDropdown.Add(req.item);
            }
        }

        itemDropdown.AddOptions(options);

        if (options.Count == 0)
        {
            options.Add("Você não tem os itens necessários...");
            itemDropdown.ClearOptions();
            itemDropdown.AddOptions(options);
            ToggleInteraction(false);
        }
        else
        {
            ToggleInteraction(true);
            itemDropdown.value = 0;
            itemDropdown.RefreshShownValue();
        }
    }

    private void UpdateProgressDisplay()
    {
        Dictionary<ItemData, int> collected = _boatProgress.GetCollectedItems();
        List<ItemRequirement> recipe = _boatProgress.recipe;

        string progressString = "<b>META DO BARCO:</b>\n";

        foreach (ItemRequirement req in recipe)
        {
            int currentAmount = 0;
            if (collected.ContainsKey(req.item))
            {
                currentAmount = collected[req.item];
            }
            progressString += $"- {req.item.itemName}: {currentAmount} / {req.amountNeeded}\n";
        }
        goalList.text = progressString;
    }

    // --- AQUI ESTÃO AS MUDANÇAS PRINCIPAIS ---
    private void OnConfirm()
    {
        if (_itemsInDropdown.Count == 0) return;

        // 1. Pega o Item Selecionado
        int selectedIndex = itemDropdown.value;
        ItemData selectedItem = _itemsInDropdown[selectedIndex];

        // 2. Valida o que foi digitado
        int amountInputted;
        if (!int.TryParse(amountInput.text, out amountInputted) || amountInputted <= 0)
        {
            // Se digitou bobagem ou zero, toca som de erro
            Debug.Log("Quantidade inválida.");
            if (AudioManager.instance != null) AudioManager.instance.PlayDepositSound(false);
            return;
        }

        // 3. CONSULTA OS LIMITES
        int amountPlayerHas = _playerInventory.GetItemCount(selectedItem);     // O que eu tenho
        int amountBoatNeeds = _boatProgress.GetAmountNeeded(selectedItem);     // O que o barco quer

        // 4. LÓGICA DE CORREÇÃO (CLAMP)
        int finalAmount = amountInputted;

        // Regra A: Não pode dar mais do que o barco precisa
        if (finalAmount > amountBoatNeeds)
        {
            finalAmount = amountBoatNeeds;
        }

        // Regra B: Não pode dar mais do que eu tenho no bolso (Melhoria Solicitada)
        if (finalAmount > amountPlayerHas)
        {
            finalAmount = amountPlayerHas;
        }

        // 5. ATUALIZA A CAIXA DE TEXTO SE O NÚMERO MUDOU
        // Isso mostra pro jogador: "Você digitou 999, mas eu corrigi para 5 pq é só o que vc tem"
        if (finalAmount != amountInputted)
        {
            amountInput.text = finalAmount.ToString();
        }

        // 6. EXECUTA A TRANSAÇÃO
        // (Como já validamos antes, aqui deve passar direto, mas mantemos o if por segurança)
        if (_playerInventory.RemoveItem(selectedItem, finalAmount))
        {
            _boatProgress.AddItemToBoat(selectedItem, finalAmount);

            // TOCA O SOM DE SUCESSO!
            if (AudioManager.instance != null) AudioManager.instance.PlayDepositSound(true);

            PopulateDropdown();
            UpdateProgressDisplay();
            amountInput.text = ""; // Limpa campo
        }
        else
        {
            // TOCA O SOM DE ERRO (Caso algo muito bizarro aconteça)
            if (AudioManager.instance != null) AudioManager.instance.PlayDepositSound(false);
        }
    }

    private void OnCancel()
    {
        CloseWindow();
    }

    private void CloseWindow()
    {
        _playerInventory = null;
        _boatProgress = null;
        amountInput.text = "";
        gameObject.SetActive(false);
    }

    private void ToggleInteraction(bool state)
    {
        itemDropdown.interactable = state;
        amountInput.interactable = state;
        confirmButton.interactable = state;
    }
}