using UnityEngine;
using TMPro; // Precisamos do TextMeshPro

public class InventoryTooltip : MonoBehaviour
{
    public static InventoryTooltip instance;

    [Header("UI Components")]
    public GameObject tooltipWindow; // O objeto pai (o painel)
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI bodyText;

    private void Awake()
    {
        instance = this;
        HideTooltip(); // Começa escondido
    }

    private void Update()
    {
        // Faz a janelinha seguir o mouse
        if (tooltipWindow.activeSelf)
        {
            // Pega a posição do mouse e adiciona um offset pra não ficar em cima do cursor
            transform.position = Input.mousePosition;
        }
    }

    public void ShowTooltip(ItemData item)
    {
        titleText.text = item.itemName;
        bodyText.text = item.description;
        tooltipWindow.SetActive(true);
    }

    public void HideTooltip()
    {
        tooltipWindow.SetActive(false);
    }
}