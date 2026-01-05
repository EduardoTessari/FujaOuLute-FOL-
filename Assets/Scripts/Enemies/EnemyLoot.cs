using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class LootTableItem
{
    public ItemData item;       // MUDOU: Agora arrasta o ScriptableObject aqui
    public int amount = 1;      // Quantos cai?
    [Range(0, 100)]
    public float dropChance = 100f; // Chance em %
}

public class EnemyLoot : MonoBehaviour
{
    [Header("Tabela de Loot")]
    [SerializeField] private List<LootTableItem> possibleLoot;

    // Função que será chamada quando o bicho morrer
    public void DropLoot()
    {
        // 1. Encontra o Jogador
        // (Nota: Se sua versão da Unity for antiga e reclamar do FindFirstObjectByType, use FindObjectOfType)
        InventoryUI playerInventory = FindFirstObjectByType<InventoryUI>();

        if (playerInventory == null)
        {
            Debug.LogWarning("EnemyLoot: Não achou o inventário do jogador!");
            return;
        }

        // 2. Roda a roleta para CADA item da lista
        foreach (LootTableItem loot in possibleLoot)
        {
            // Segurança: se esqueceu de arrastar o item no Inspector, pula
            if (loot.item == null) continue;

            float roll = Random.Range(0f, 100f);

            // Se o número for menor que a chance, GANHOU!
            if (roll <= loot.dropChance)
            {
                // Agora essa função funciona pois o AddItem espera (ItemData, int)
                playerInventory.AddItem(loot.item, loot.amount);

                Debug.Log($"SORTE! Dropou: {loot.item.itemName}");
            }
        }
    }
}