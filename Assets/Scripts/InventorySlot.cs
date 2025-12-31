using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems; // IMPORTANTE: Necessário para detectar o mouse!

// Adicionamos as interfaces na linha abaixo
public class InventorySlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Componentes da UI")]
    public Image iconImage;
    public TextMeshProUGUI amountText;

    // Guardamos o dado aqui para saber o que mostrar no tooltip
    private ItemData _currentItem;

    public void SetupSlot(ItemData itemData, int amount)
    {
        _currentItem = itemData; // Guardamos a referência

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
        _currentItem = null; // Limpa a referência
        iconImage.sprite = null;
        iconImage.enabled = false;
        amountText.enabled = false;
    }

    // --- MÁGICA DO MOUSE ---

    // O mouse entrou no quadrado?
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (_currentItem != null)
        {
            InventoryTooltip.instance.ShowTooltip(_currentItem);
        }
    }

    // O mouse saiu do quadrado?
    public void OnPointerExit(PointerEventData eventData)
    {
        InventoryTooltip.instance.HideTooltip();
    }
}