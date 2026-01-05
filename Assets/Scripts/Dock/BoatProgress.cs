using UnityEngine;
using System.Collections.Generic;

// Classe auxiliar para definir o que o barco precisa
[System.Serializable]
public class ItemRequirement
{
    public ItemData item;      // MUDANÇA: Agora arrasta o ScriptableObject aqui!
    public int amountNeeded;   // Quanto precisa?
}

public class BoatProgress : MonoBehaviour
{
    [Header("A Receita do Barco")]
    // Lista de materiais necessários (Configure no Inspector)
    public List<ItemRequirement> recipe = new List<ItemRequirement>();

    // Onde guardamos o que já foi entregue
    private Dictionary<ItemData, int> collectedItems = new Dictionary<ItemData, int>();

    // --- FUNÇÕES PÚBLICAS (Chamadas pela Doca) ---

    // 1. Recebe o item da Doca
    public void AddItemToBoat(ItemData item, int amount)
    {
        if (collectedItems.ContainsKey(item))
        {
            collectedItems[item] += amount;
        }
        else
        {
            collectedItems.Add(item, amount);
        }

        Debug.Log($"Barco recebeu {amount} de {item.itemName}.");
        CheckIfComplete(); // Verifica se terminou o barco
    }

    // 2. Diz para a Doca quanto ainda falta de um item específico
    public int GetAmountNeeded(ItemData item)
    {
        // Procura o item na receita
        foreach (var req in recipe)
        {
            if (req.item == item)
            {
                int currentAmount = 0;
                if (collectedItems.ContainsKey(item))
                {
                    currentAmount = collectedItems[item];
                }

                int missing = req.amountNeeded - currentAmount;
                return Mathf.Max(0, missing); // Retorna 0 se já tiver passado do total
            }
        }
        return 0; // Se o item não está na receita, precisa de 0.
    }

    // 3. Retorna tudo que já foi coletado (para a Doca mostrar na lista)
    public Dictionary<ItemData, int> GetCollectedItems()
    {
        return collectedItems;
    }

    // --- VERIFICAÇÃO FINAL ---
    private void CheckIfComplete()
    {
        bool allComplete = true;

        foreach (var req in recipe)
        {
            int current = 0;
            if (collectedItems.ContainsKey(req.item))
            {
                current = collectedItems[req.item];
            }

            if (current < req.amountNeeded)
            {
                allComplete = false;
                break;
            }
        }

        if (allComplete)
        {
            Debug.Log("PARABÉNS! O BARCO ESTÁ PRONTO!");
            // Aqui você chamaria o GameManager para vencer o jogo ou tocar uma cutscene
        }
    }
}