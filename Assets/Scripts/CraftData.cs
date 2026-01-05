using System.Collections.Generic;
using UnityEngine;

// 1. Classe auxiliar para Ingredientes
[System.Serializable]
public class Ingrediente
{
    public ItemData item;       // O Item (Madeira, Ferro, etc)
    public int quantidade = 1;  // Quantos precisa
}

// 2. Classe Principal (Agora chamada CraftData)
[CreateAssetMenu(fileName = "Nova Receita", menuName = "Sistema Craft/Receita")]
public class CraftData : ScriptableObject
{
    public string nomeDaReceita; // Ex: "Espada de Ferro"

    // Lista de ingredientes
    public List<Ingrediente> ingredientesNecessarios;

    // Item final gerado
    public ItemData itemResultado;
    public int quantidadeResultado = 1;
}