using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    [Header("Atributos")]
    [SerializeField] private float moveSpeed = 2.5f;

    private Transform _playerTarget;
    private Rigidbody2D _rb;
    private bool _isChasing = false;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    // --- A LÓGICA DO "SENTIDO DE ARANHA" ---

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            // Viu o jogador! Começa a caça.
            _playerTarget = collision.transform;
            _isChasing = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            // Jogador fugiu. Para de caçar.
            _isChasing = false;
            _rb.linearVelocity = Vector2.zero; // Freia o bicho imediatamente
        }
    }

    // --- A LÓGICA DO MOVIMENTO ---

    private void FixedUpdate()
    {
        if (_isChasing && _playerTarget != null)
        {
            // 1. Calcula a direção
            Vector2 direction = (_playerTarget.position - transform.position).normalized;

            // 2. Move
            _rb.MovePosition(_rb.position + direction * moveSpeed * Time.fixedDeltaTime);

            // (Opcional) Girar o sprite
            if (direction.x > 0) transform.localScale = new Vector3(1, 1, 1);
            else if (direction.x < 0) transform.localScale = new Vector3(-1, 1, 1);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Verifica se encostou no Player
        if (collision.gameObject.CompareTag("Player"))
        {
            // Tenta pegar o script de vida do Player
            HealthSystem playerHealth = collision.gameObject.GetComponent<HealthSystem>();

            if (playerHealth != null)
            {
                // CAUSA DANO!
                playerHealth.TakeDamage(1);

                // (Opcional) Empurrãozinho para trás (Knockback) para não dar dano todo frame
                // Mas por enquanto, só o dano basta.
            }
        }
    }
}
