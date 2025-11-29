using UnityEngine;

public class DamageDealer : MonoBehaviour
{
    [Header("Configuração")]
    [SerializeField] private int damage = 1;
    [SerializeField] private string targetTag = "Enemy"; // Só bate em quem tiver essa tag!

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 1. Segurança: Verifica se não bateu no próprio Player ou em parede
        // (Dica: Vá no Sapo e coloque a Tag "Enemy" nele!)
        if (collision.CompareTag(targetTag))
        {
            // 2. Pega a vida do alvo
            HealthSystem targetHealth = collision.GetComponent<HealthSystem>();

            if (targetHealth != null)
            {
                // 3. CAUSA A DOR!
                targetHealth.TakeDamage(damage);
                Debug.Log("TOMA ESSA! Sapo ferido.");
            }
        }
    }
}
