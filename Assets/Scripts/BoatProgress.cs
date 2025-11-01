using System.Collections.Generic; // Precisamos disso para Dicionários e Listas!
using UnityEngine;
using UnityEngine.UIElements;

// --- PASSO 1: A "RECEITA" ---
// Esta é uma pequena "classe auxiliar" que vamos usar.
// Ela nos permite criar uma "lista de compras" no Inspector.
[System.Serializable] // Isso faz ela aparecer no Inspector da Unity
public class ItemRequirement
{
    public ItemList item;       // O item que precisamos (do nosso "cardápio")
    public int amountNeeded;  // A quantidade
}


// --- PASSO 2: O SCRIPT PRINCIPAL ---
public class BoatProgress : MonoBehaviour
{
    public GameObject winPanel, DockProgress; // Painel que aparece quando o barco está completo

    [Header("Receita do Barco")]
    // Esta é a "lista de compras" que você vai preencher no Inspector!
    public List<ItemRequirement> recipe = new List<ItemRequirement>();

    [Header("Progresso Atual")]
    // O "carrinho de compras" (o que já coletamos), agora é um Dicionário.
    [SerializeField]
    private Dictionary<ItemList, int> itemsCollected = new Dictionary<ItemList, int>();

    /**
     * Esta é a ÚNICA função pública que a Doca vai chamar.
     * Ela é inteligente: recebe o item, guarda e checa se vencemos.
     */
    public void AddItemToBoat(ItemList item, int amount)
    {
        if (amount <= 0) return;

        // Adiciona o item ao nosso "carrinho"
        if (itemsCollected.ContainsKey(item))
        {
            itemsCollected[item] += amount;
        }
        else
        {
            itemsCollected.Add(item, amount);
        }

        Debug.Log($"ENTREGUE AO BARCO: {amount} de {item}! Progresso total: {itemsCollected[item]}");

        // Após cada entrega, checa se já vencemos
        CheckWinCondition();
    }

    // Uma função pública para obter o progresso atual (útil para UI)
    public Dictionary<ItemList, int> GetCollectedItems()
    {
        return itemsCollected;
    }

    /**
     * Uma função privada que checa se a "lista de compras" bate com o "carrinho".
     */
    private void CheckWinCondition()
    {
        // Vamos checar CADA item da nossa receita
        foreach (ItemRequirement req in recipe)
        {
            // 1. Checa se já coletamos este item alguma vez
            if (!itemsCollected.ContainsKey(req.item))
            {
                // Se o item nem está no dicionário, ainda não vencemos.
                return;
            }

            // 2. Checa se a quantidade que coletamos é o suficiente
            if (itemsCollected[req.item] < req.amountNeeded)
            {
                // Se a quantidade for menor, ainda não vencemos.
                return;
            }
        }

        if (winPanel != null)
        {
            if (DockProgress != null)
            {
                DockProgress.SetActive(false); // DESLIGA A TELA DE PROGRESSO DO BARCO
            }

            winPanel.SetActive(true); // LIGA A TELA DE VITÓRIA
        }

        // AQUI SIM NÓS PAUSAMOS O JOGO!
        Time.timeScale = 0f; // O jogo acabou, pode pausar.


        // Se o loop terminou e não saímos em nenhum "return"…
        // ...significa que temos TODOS os itens na quantidade certa!
        Debug.LogWarning("BARCO CONSTRUÍDO! TODOS OS RECURSOS COLETADOS! VENCEU!");
        
    }
}