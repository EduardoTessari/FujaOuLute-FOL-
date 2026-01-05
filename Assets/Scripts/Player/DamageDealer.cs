using UnityEngine;

public class DamageDealer : MonoBehaviour
{
    [Header("Configuração")]
    [SerializeField] private string targetTag = "Enemy"; // Quem ele deve machucar

    // Variável privada para guardar o dano que a arma mandou
    private int _currentDamage = 1;

    // A arma (Sword/Bow) chama isso para dizer: "Ei, cause X de dano!"
    public void SetDamage(int amount)
    {
        _currentDamage = amount;
    }
    // --------------------------------------------

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 1. Segurança: Verifica se bateu no alvo certo (Inimigo)
        if (collision.CompareTag(targetTag))
        {
            // 2. Pega a vida do alvo
            HealthSystem targetHealth = collision.GetComponent<HealthSystem>();

            if (targetHealth != null)
            {
                // 3. CAUSA A DOR! (Usando o valor que recebemos via SetDamage)
                targetHealth.TakeDamage(_currentDamage);
            }

            // (Opcional) Se for um projétil, ele se destrói aqui também
            // Mas geralmente o script Projectile cuida disso.
        }
    }
}