using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class InventorySlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [Header("Componentes da UI")]
    public Image iconImage;
    public TextMeshProUGUI amountText;

    private ItemData _currentItem;
    private bool _isInteractable = true; // NOVA VARIÁVEL DE CONTROLE

    // --- MODO 1: USADO NO INVENTÁRIO (NORMAL) ---
    public void SetupSlot(ItemData itemData, int amount)
    {
        _currentItem = itemData;
        _isInteractable = true; // Permite clicar e abrir menu

        iconImage.sprite = itemData.icon;
        iconImage.enabled = true;

        // Lógica original: esconde número 1
        if (amount > 1)
        {
            amountText.text = amount.ToString();
            amountText.enabled = true;
        }
        else
        {
            amountText.enabled = false;
        }
    }

    // --- MODO 2: USADO NO CRAFT (NOVO) ---
    public void SetupForCrafting(ItemData itemData, int amount)
    {
        _currentItem = itemData;
        _isInteractable = false; // TRAVA O CLIQUE (Não abre menu de ação)

        if (itemData != null)
        {
            iconImage.sprite = itemData.icon;
            iconImage.enabled = true;
        }

        // Em receitas, é legal mostrar o número mesmo se for 1 (Ex: precisa de "1")
        amountText.text = amount.ToString();
        amountText.enabled = true;
    }

    public void ClearSlot()
    {
        _currentItem = null;
        _isInteractable = true;
        iconImage.sprite = null;
        iconImage.enabled = false;
        amountText.enabled = false;
    }

    // --- MÁGICA DO MOUSE ---

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (_currentItem != null)
        {
            // O Tooltip continua funcionando no Craft (é bom pra ver o nome do item)
            InventoryTooltip.instance.ShowTooltip(_currentItem);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        InventoryTooltip.instance.HideTooltip();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        // NOVA TRAVA DE SEGURANÇA
        if (!_isInteractable) return;

        if (_currentItem != null)
        {
            InventoryTooltip.instance.HideTooltip();
            Debug.Log("Abrindo menu para: " + _currentItem.itemName);
            ItemActionMenu.Instance.OpenMenu(_currentItem, transform.position);
        }
    }
}