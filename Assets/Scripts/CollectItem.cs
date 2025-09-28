using System.Collections;
using UnityEngine;
using UnityEngine.UI;

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


    private void Awake()
    {
        //successZone = GetComponent<SucessZone>();
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
        StartCoroutine(SkillCheckRoutine());
        _elapsedTime = 0f;

        while (_elapsedTime < _collectTime)
        {
            if (_playerInRange != null && _playerInRange.MoveInput != Vector2.zero)
            {
                progressBar.gameObject.SetActive(false);
                skillCheckGroup.SetActive(false);
                StopCoroutine(SkillCheckRoutine()); // Importante: parar a outra coroutine também!
                _canCollect = true;
                yield break;
            }
            _elapsedTime += Time.deltaTime;
            progressBar.value = Mathf.Clamp01(_elapsedTime / _collectTime);
            yield return null;
        }

        progressBar.gameObject.SetActive(false);
        skillCheckGroup.SetActive(false);
        Debug.Log("Item Collected!");
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
                    Debug.Log("SKILL CHECK SUCESSO! (Com Collider)");
                    _elapsedTime += bonusTime;
                }
                else
                {
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