using UnityEngine;
using UnityEngine.UI; // Para mexer na Image
using TMPro;          // Para mexer no Texto

public class InventorySlot : MonoBehaviour
{
    [Header("Componentes da UI")]
    public Image iconImage;
    public TextMeshProUGUI amountText;

    // Essa função vai ser chamada pelo Gerente de UI para preencher os dados
    public void SetupSlot(ItemData itemData, int amount)
    {
        // 1. Troca a foto
        iconImage.sprite = itemData.icon;
        iconImage.enabled = true; // Garante que a imagem apareça

        // 2. Atualiza o número
        if (amount > 1)
        {
            amountText.text = amount.ToString();
            amountText.enabled = true;
        }
        else
        {
            // Se for só 1 (tipo espada), esconde o número pra ficar limpo
            amountText.enabled = false;
        }
    }
}

