using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

// 1. ADICIONEI O "IPointerClickHandler" AQUI NA LISTA DE INTERFACES
public class InventorySlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [Header("Componentes da UI")]
    public Image iconImage;
    public TextMeshProUGUI amountText;

    private ItemData _currentItem;

    public void SetupSlot(ItemData itemData, int amount)
    {
        _currentItem = itemData;

        iconImage.sprite = itemData.icon;
        iconImage.enabled = true;

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

    public void ClearSlot()
    {
        _currentItem = null;
        iconImage.sprite = null;
        iconImage.enabled = false;
        amountText.enabled = false;
    }

    // --- MÁGICA DO MOUSE ---

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (_currentItem != null)
        {
            InventoryTooltip.instance.ShowTooltip(_currentItem);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        InventoryTooltip.instance.HideTooltip();
    }

    // 2. AQUI ESTÁ A NOVA MÁGICA DO CLIQUE
    public void OnPointerClick(PointerEventData eventData)
    {
        // Só faz algo se tiver item no slot
        if (_currentItem != null)
        {
            // A Regra de Ouro: Fecha o Tooltip imediatamente pra não atrapalhar
            InventoryTooltip.instance.HideTooltip();

            // Chama o Menu de Ação passando o Item e a Posição do Slot
            Debug.Log("Abrindo menu para: " + _currentItem.itemName);
            ItemActionMenu.Instance.OpenMenu(_currentItem, transform.position);
        }
    }
}