using UnityEngine;

public class PLayerControler : MonoBehaviour
{
    [SerializeField] private float speed = 4.0f;
    private Vector2 _moveInput;
    private Rigidbody2D _rb;
    private Animator _animator;
    private PlayerMoves _playerMoves;

    [Header("Audio Settings")]
    [SerializeField] private float stepInterval = 0.4f; // Ajuste isso no Inspector! (ex: 0.4 ou 0.5)
    private float _stepTimer;



    private void Awake()
    {
        _playerMoves = new PlayerMoves();
        _rb = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
    }

    private void OnEnable()
    {
        _playerMoves.Enable();
    }

    private void Update()
    {
        PlayerInput();   
    }

    private void FixedUpdate()
    {
        Move();
    }

    private void PlayerInput()
    {
        _moveInput = _playerMoves.Movement.Move.ReadValue<Vector2>();
        playerFlip();

    }

    private void Move()
    {
        // 1. Aplica o movimento físico
        _rb.MovePosition(_rb.position + _moveInput * speed * Time.fixedDeltaTime);

        // 2. Verifica se está se movendo para Animação e Som
        if (_moveInput != Vector2.zero)
        {
            _animator.SetInteger("State", 1);

            // --- LÓGICA DO TIMER DE PASSOS ---
            _stepTimer += Time.deltaTime; // O relógio conta...

            if (_stepTimer >= stepInterval)
            {
                // Hora do passo! Chama o Maestro.
                AudioManager.instance.PlayFootstep();

                // Zera o relógio para contar o próximo passo
                _stepTimer = 0f;
            }
            // ---------------------------------
        }
        else
        {
            _animator.SetInteger("State", 0);

            // Reset Inteligente:
            // Deixamos o timer "cheio" para que, assim que você voltar a andar,
            // o primeiro passo saia imediatamente (sem delay).
            _stepTimer = stepInterval;
        }

    }

    private void playerFlip()
    {
        if (_moveInput.x < 0)
        {
            gameObject.transform.localScale = new Vector2(-1, 1);
        }
        else if (_moveInput.x > 0)
        {
            transform.localScale = new Vector2(1, 1);
        }
    }

    public Vector2 MoveInput { get { return _moveInput; } }
}
