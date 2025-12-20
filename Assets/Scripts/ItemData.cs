using UnityEngine;

// A linha mágica! Isso cria uma opção no menu do botão direito da Unity.
[CreateAssetMenu(fileName = "New Item Data", menuName = "FOL/Item Data")]
public class ItemData : ScriptableObject // <-- Note que NÃO é MonoBehaviour!
{
    [Header("Identidade")]
    public string itemName;      // Nome bonito (ex: "Madeira de Carvalho")
    public ItemList itemType;    // O link com seu Enum (ex: ItemList.Wood)

    [Header("Visual")]
    public Sprite icon;          // A foto que vai aparecer no inventário

    [Header("Detalhes")]
    [TextArea(3, 10)]            // Cria uma caixa de texto maior no Inspector
    public string description;   // Ex: "Usada para construir barcos e fogueiras."

    [Header("Configurações")]
    public bool isStackable = true; // Se pode juntar vários no mesmo slot
    public int maxStackSize = 99;   // Máximo por slot
}