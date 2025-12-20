using UnityEngine;
using UnityEngine.Events;

public class HealthSystem : MonoBehaviour
{
    [Header("Configurações")]
    [SerializeField] private int maxHealth = 5;
    [SerializeField] private int currentHealth;

    [Header("Eventos")]
    public UnityEvent OnDeath;

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
        // Dispara qualquer coisa conectada a este evento (Loot, Som, Partícula)
        OnDeath?.Invoke();

        // Destrói o objeto (Sapo)
        Destroy(gameObject);
    }
}
