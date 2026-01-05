using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.PlayerSettings;
using static UnityEditor.Timeline.Actions.MenuPriority;

public class InventoryUI : MonoBehaviour
{
    [SerializeField] GameObject UIGameObject;

    [Header("Configuração Visual")]
    public GameObject slotPrefab;   // Arraste o prefab do botãozinho aqui
    public Transform slotsParent;   // Arraste o Grid/Content aqui

    [Header("Seus Itens (Dados)")]
    // Agora os dados moram aqui!
    public List<ListItem> inventoryItems = new List<ListItem>();

    // --- 1. A FUNÇÃO QUE O COLLECTITEM ESTÁ PROCURANDO ---
    public void AddItem(ItemData data, int amount)
    {
        // Verifica se já tem o item
        ListItem itemExistente = null;
        foreach (ListItem item in inventoryItems)
        {
            if (item.data == data)
            {
                itemExistente = item;
                break;
            }
        }

        // Soma ou Cria novo
        if (itemExistente != null)
        {
            itemExistente.stackSize += amount;
            Debug.Log($"InventoryUI: Somou +{amount} de {data.itemName}");
        }
        else
        {
            inventoryItems.Add(new ListItem(data, amount));
            Debug.Log($"InventoryUI: Novo item {data.itemName}");
        }

        UpdateDisplay(); // Atualiza a tela
    }

    // --- 2. A FUNÇÃO QUE A DOCA/CRAFTING VAI PRECISAR ---
    public bool RemoveItem(ItemData data, int amount)
    {
        ListItem itemEncontrado = null;
        foreach (var item in inventoryItems)
        {
            if (item.data == data)
            {
                itemEncontrado = item;
                break;
            }
        }

        if (itemEncontrado != null && itemEncontrado.stackSize >= amount)
        {
            itemEncontrado.stackSize -= amount;

            if (itemEncontrado.stackSize <= 0)
            {
                inventoryItems.Remove(itemEncontrado);
            }

            UpdateDisplay();
            return true;
        }
        return false;
    }

    // --- 3. A FUNÇÃO DE CONSULTA (Pra Doca saber o que você tem) ---
    public int GetItemCount(ItemData data)
    {
        foreach (ListItem item in inventoryItems)
        {
            if (item.data == data)
            {
                return item.stackSize;
            }
        }
        return 0;
    }

    // --- 4. A ATUALIZAÇÃO VISUAL (Que ele já fazia antes) ---
    public void UpdateDisplay()
    {
        // Limpa slots antigos
        foreach (Transform child in slotsParent)
        {
            Destroy(child.gameObject);
        }

        // Cria slots novos baseados na lista DESTE script
        foreach (ListItem item in inventoryItems)
        {
            GameObject newSlot = Instantiate(slotPrefab, slotsParent);

            InventorySlot slotScript = newSlot.GetComponent<InventorySlot>();
            if (slotScript != null)
            {
                slotScript.SetupSlot(item.data, item.stackSize);
            }
        }
    }

    public void OpenInventoryUI()
    {
        // "Defina o ativo como: O CONTRÁRIO de como ele está agora"
        UIGameObject.SetActive(!UIGameObject.activeSelf);
    }

}

// --- CLASSE DE DADOS (Agora mora aqui) ---
[System.Serializable]
public class ListItem
{
    public ItemData data;
    public int stackSize;

    public ListItem(ItemData d, int q)
    {
        data = d;
        stackSize = q;
    }
}