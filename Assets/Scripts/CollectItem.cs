using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic; // Precisamos disso para Dicionários e Listas!

public class CollectItem : MonoBehaviour
{
    [Header("Collection Settings")]
    [SerializeField] float _collectTime = 10f; 

    [Header("UI References")]
    public Slider progressBar;
    public Slider skillCheckBar;
    public GameObject skillCheckGroup;
    public Image successZoneImage;

    [Header("Skill Check Settings")]
    public KeyCode skillCheckKey = KeyCode.F;
    public float skillCheckDuration = 10f;
    public float skillCheckSpeed = 0.5f; // <-- NOSSA NOVA VARIÁVEL DE VELOCIDADE
    public float bonusTime = 6f;

    // Variáveis Privadas
    private bool _canCollect = false;
    private PLayerControler _playerInRange = null;
    private float _elapsedTime;

    public SucessZone successZone;

    [Header ("Audios Settings")]
    [SerializeField] AudioSource _treeAudioSource;
    [SerializeField] AudioClip _collectAudioClip;
    [SerializeField] AudioClip _successAudioClip;
    [SerializeField] AudioClip _FailAudioClip;

    [Header("Item Drop Settings")]
    public ItemList itemToGive; // <-- AQUI! Crie este campo.
    public int amountToGive = 1; // Quantidade que ela dá



    private void Awake()
    {
        _treeAudioSource = GetComponent<AudioSource>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            _playerInRange = collision.GetComponent<PLayerControler>();
            _canCollect = true;
        }
    }

    private void Update()
    {
        if (_playerInRange != null && _canCollect && Input.GetKeyDown(KeyCode.E))
        {
            _canCollect = false;
            StartCoroutine(CollectTime());
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            _playerInRange = null;
            _canCollect = false;
        }
    }

    IEnumerator CollectTime()
    {
        progressBar.gameObject.SetActive(true);
        progressBar.value = 0;
        _treeAudioSource.clip = _collectAudioClip;
        _treeAudioSource.loop = true;
        _treeAudioSource.Play();
        StartCoroutine(SkillCheckRoutine());
        _elapsedTime = 0f;

        while (_elapsedTime < _collectTime)
        {
            if (_playerInRange != null && _playerInRange.MoveInput != Vector2.zero)
            {
                progressBar.gameObject.SetActive(false);
                skillCheckGroup.SetActive(false);
                _treeAudioSource.Stop();
                _treeAudioSource.loop = false;
                StopCoroutine(SkillCheckRoutine()); // Importante: parar a outra coroutine também!
                _canCollect = true;
                yield break;
            }
            _elapsedTime += Time.deltaTime;
            progressBar.value = Mathf.Clamp01(_elapsedTime / _collectTime);
            yield return null;
        }

        _treeAudioSource.Stop();
        _treeAudioSource.loop = false;
        progressBar.gameObject.SetActive(false);
        skillCheckGroup.SetActive(false);
        _playerInRange.GetComponent<Inventory>().AddItem(itemToGive, amountToGive);
        //Destroy(gameObject);
    }

    IEnumerator SkillCheckRoutine()
    {
        Debug.Log("Skill check started!");
        skillCheckGroup.SetActive(true);

        float timer = 0f;                   // Cronômetro para a DURAÇÃO total do skill check
        float oscillationTimer = 0f;        // Cronômetro para a OSCILAÇÃO da barra
        bool skillCheckUsed = false;

        while (timer < skillCheckDuration && !skillCheckUsed)
        {
            //Debug.Log($"Skill Check está VIVA! Timer: {timer}");
            // 1. O timer da oscilação continua crescendo para alimentar a onda
            oscillationTimer += Time.deltaTime;

            // 2. A matemática final usando Cosseno para uma curva suave de 0 a 1 e de volta a 0
            float oscillation = (-Mathf.Cos(oscillationTimer * skillCheckSpeed) + 1f) / 2f;
            skillCheckBar.value = oscillation;

            // 3. Checagem do input do jogador
            if (Input.GetKeyDown(skillCheckKey))
            {
                skillCheckUsed = true;

                // A NOVA LÓGICA DE ACERTO, MUITO MAIS SIMPLES!
                if (successZone.isBarInside)
                {
                    _treeAudioSource.PlayOneShot(_successAudioClip);
                    Debug.Log("SKILL CHECK SUCESSO! (Com Collider)");
                    _elapsedTime += bonusTime;
                }
                else
                {
                    _treeAudioSource.PlayOneShot(_FailAudioClip);
                    Debug.Log("SKILL CHECK FALHA! (Com Collider)");
                }
            }

            // 4. O timer principal continua contando para encerrar a rotina no tempo certo
            timer += Time.deltaTime;
            yield return null;
        }

        Debug.Log("Skill check finished.");
        skillCheckGroup.SetActive(false); // Garante que a UI suma no final
    }
}