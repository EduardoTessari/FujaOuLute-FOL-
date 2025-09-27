using UnityEngine;

public class PLayerControler : MonoBehaviour
{
    [SerializeField] private float speed = 4.0f;
    [SerializeField] AudioSource _audioSource;
    private Vector2 _moveInput;
    private Rigidbody2D _rb;
    private Animator _animator;
    private PlayerMoves _playerMoves;

    

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
        _rb.MovePosition(_rb.position + _moveInput * speed * Time.fixedDeltaTime);
        
        if (_moveInput != Vector2.zero)
        {
            _animator.SetInteger("State", 1);
            if (!_audioSource.isPlaying)
            {
                _audioSource.Play();
            }
        }
        else
        {
            _animator.SetInteger("State", 0);
            if (_audioSource.isPlaying)
            {
                _audioSource.Stop();
            }
            
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
