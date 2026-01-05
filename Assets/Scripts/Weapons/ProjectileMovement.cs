using UnityEngine;

public class ProjectileMovement : MonoBehaviour
{
    [Header("Configurações")]
    [SerializeField] private float speed = 10f;
    [SerializeField] private float lifeTime = 3f;

    private Rigidbody2D _rb;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    // NÃO USAMOS MAIS O START! 
    // Usamos esta função pública que a Arma vai chamar.
    public void Launch(Vector2 direction)
    {
        // 1. Configura a morte por tempo
        Destroy(gameObject, lifeTime);

        // 2. Aplica a velocidade na direção que recebemos
        _rb.linearVelocity = direction * speed;

        // 3. (Opcional) O Polimento Visual:
        // Se a direção for para a esquerda, a gente vira o sprite da flecha
        if (direction.x < 0)
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy") || collision.CompareTag("Wall"))
        {
            Destroy(gameObject);
        }
    }
}
