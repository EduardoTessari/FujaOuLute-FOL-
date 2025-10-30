using UnityEngine;
using System.Collections.Generic; // Importante! Precisamos disso para o Dicionário

public class Inventory : MonoBehaviour
{
    // O NOVO INVENTÁRIO "INTELIGENTE"
    // Ele liga um "ItemType" (do nosso cardápio) a um "int" (a quantidade).
    // O [SerializeField] nos deixa ver no Inspector, o que é ótimo para debug.
    [SerializeField]
    private Dictionary<ItemList, int> items = new Dictionary<ItemList, int>();

    // --- FUNÇÕES PÚBLICAS ATUALIZADAS ---

    /**
     * Adiciona uma certa quantidade de um item ao inventário.
     */
    public void AddItem(ItemList item, int amount)
    {
        if (amount <= 0) return; // Segurança

        // Se já temos esse item, apenas somamos a quantidade
        if (items.ContainsKey(item))
        {
            items[item] += amount;
        }
        // Se é a primeira vez, adicionamos o item ao dicionário com a quantidade
        else
        {
            items.Add(item, amount);
        }
        Debug.Log($"Pegou {amount} de {item}! Total agora: {items[item]}");
    }

    /**
     * Tenta remover uma quantidade específica de um item.
     * Retorna 'true' se foi bem-sucedido, 'false' se não.
     */
    public bool RemoveItem(ItemList item, int amount)
    {
        // Checa se temos o item E se temos a quantidade suficiente
        if (items.ContainsKey(item) && items[item] >= amount)
        {
            items[item] -= amount;
            Debug.Log($"Removeu {amount} de {item}. Sobraram {items[item]}");

            // Se zerar, podemos remover o item da lista (opcional, mas limpo)
            if (items[item] == 0)
            {
                items.Remove(item);
            }
            return true; // Sucesso! Conseguiu remover.
        }

        Debug.Log($"Tentou remover {amount} de {item}, mas não foi possível.");
        return false; // Falha! Não tinha o item ou a quantidade.
    }

    /**
     * Apenas "lê" a quantidade de um item. (Para a UI da Doca)
     */
    public int GetItemCount(ItemList item)
    {
        if (items.ContainsKey(item))
        {
            return items[item];
        }
        return 0; // Não tem esse item no inventário
    }

    /**
     * Retorna o inventário inteiro. (Para a UI da Doca popular o Dropdown)
     */
    public Dictionary<ItemList, int> GetCurrentInventory()
    {
        return items;
    }
}