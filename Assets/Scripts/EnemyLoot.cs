using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class LootTableItem
{
    public ItemList item;       // Qual item? (Carne, Veneno)
    public int amount = 1;      // Quantos?
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
        // 1. Encontra o Jogador (precisamos do inventário dele)
        // (Como o loot é "mágico" e vai direto pra mochila, precisamos achar a mochila)
        Inventory playerInventory = FindFirstObjectByType<Inventory>();

        if (playerInventory == null) return;

        // 2. Roda a roleta para CADA item da lista
        foreach (LootTableItem loot in possibleLoot)
        {
            // Gera um número aleatório entre 0 e 100
            float roll = Random.Range(0f, 100f);
            Debug.Log($"Rolou {roll} para {loot.item} com chance de {loot.dropChance}%");

            // Se o número for menor que a chance, GANHOU!
            // Ex: Chance 25%. Se cair 10, ganhou. Se cair 50, perdeu.
            if (roll <= loot.dropChance)
            {
                playerInventory.AddItem(loot.item, loot.amount);
                
                Debug.Log($"SORTE! Dropou: {loot.item}");
                

                // (Futuro: Aqui chamaremos o Texto Flutuante)
            }
        }
    }
}