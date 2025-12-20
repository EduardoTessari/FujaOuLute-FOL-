using UnityEngine;
using System.Collections.Generic; // Necessário para usar LISTAS

public class InventoryUI : MonoBehaviour
{
    // O Singleton (Para facilitar o acesso de qualquer lugar)
    public static InventoryUI instance;

    [Header("Referências")]
    public Transform gridContainer;  // Onde os slots vão nascer (o objeto com Grid Layout Group)
    public GameObject slotPrefab;    // O molde do slot que criamos

    private void Awake()
    {
        instance = this;
    }

    // Essa é a função que vamos chamar toda vez que o inventário mudar
    public void UpdateDisplay(List<ListItem> inventoryList)
    {
        // 1. Limpeza: Destroi todos os slots antigos para não duplicar
        foreach (Transform child in gridContainer)
        {
            Destroy(child.gameObject);
        }

        // 2. Criação: Para cada item na lista do jogador...
        foreach (ListItem item in inventoryList)
        {
            // Cria o slot dentro do Grid
            GameObject newSlot = Instantiate(slotPrefab, gridContainer);

            // Pega o script do slot e preenche os dados
            InventorySlot slotScript = newSlot.GetComponent<InventorySlot>();
            if (slotScript != null)
            {
                slotScript.SetupSlot(item.data, item.stackSize);
            }
        }
    }
}

// --- CLASSE AUXILIAR (Pode ficar aqui mesmo) ---
// Essa classe representa "O que tem no bolso do jogador"
[System.Serializable] // Isso faz aparecer no Inspector pra gente testar!
public class ListItem
{
    public ItemData data;   // Qual é o item (Madeira, Espada...)
    public int stackSize;   // Quantos tem (1, 10, 99...)

    // Construtor rápido
    public ListItem(ItemData d, int q)
    {
        data = d;
        stackSize = q;
    }
}