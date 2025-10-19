using UnityEngine;

public class Inventory : MonoBehaviour
{
    [SerializeField] int woodCount = 0;

public void AddWood(int amount)
    {
        if (amount <= 0) return; // Segurança contra valores negativos

        woodCount += amount;
        Debug.Log($"Pegou {amount} de madeira! Total agora: {woodCount}");
        // Futuramente: tocar som de coleta aqui
    }

    public void DepositWood(int amount)
    {
        if (amount <= 0) return; // Segurança contra valores negativos

        if (amount > woodCount)
        {
            Debug.Log("Não há madeira suficiente para depositar!");
            return;
        }
        woodCount -= amount;
        Debug.Log($"Removeu {amount} de madeira! Total agora: {woodCount}");
        // Futuramente: tocar som de remoção aqui
    }
}


