using UnityEngine;

public class HealthSystem : MonoBehaviour
{
    [Header("Configurações")]
    [SerializeField] private int maxHealth = 5;
    [SerializeField] private int currentHealth;

    private void Start()
    {
        currentHealth = maxHealth; // Começa com vida cheia
    }

    public void TakeDamage(int damageAmount)
    {
        currentHealth -= damageAmount;
        Debug.Log($"{gameObject.name} tomou {damageAmount} de dano! Vida restante: {currentHealth}");

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log($"{gameObject.name} MORREU!");
        // Futuramente: Game Over ou Respawn
        // gameObject.SetActive(false); // Desativa por enquanto
    }
}
