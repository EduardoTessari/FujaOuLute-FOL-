using System.Collections;
using UnityEngine.UI;
using UnityEngine;

public class CollectItem : MonoBehaviour
{
    [SerializeField] bool _canCollect = false;
    [SerializeField] float _collectTime = 3f; // Time in seconds to collect the item
    private PLayerControler _playerInRange = null;

    private float successMinValue;
    private float successMaxValue;
    private float _elapsedTime;

    [Header("UI References")]
    public Slider progressBar; // Reference to the UI Slider for progress indication
    public Slider skillCheckBar;
    public GameObject skillCheckGroup;
    public Image successZoneImage;

    
    [Header("Skill Check Settings")]
    public KeyCode skillCheckKey = KeyCode.F; // Qual tecla o jogador aperta
    public float skillCheckDuration = 2.5f;   // Quanto tempo o skill check fica na tela
    public float bonusTime = 1.0f;            // Bônus de tempo em segundos se acertar


    private void Awake()
    {
        if (successZoneImage != null)
        {
            // 1. Pegamos o RectTransform e guardamos em uma NOVA variável temporária
            RectTransform successZoneRect = successZoneImage.GetComponent<RectTransform>();

            // 2. Agora usamos essa nova variável para ler as posições
            successMinValue = successZoneRect.anchorMin.x;
            successMaxValue = successZoneRect.anchorMax.x;

            Debug.Log($"Zona de acerto definida entre {successMinValue} e {successMaxValue}");
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            _playerInRange = collision.GetComponent<PLayerControler>();
            _canCollect = true;
            Debug.Log("Press E to collect the item.");
        }
    }

    private void Update() // Vamos usar Update para o Input
    {
        if (_playerInRange != null && _canCollect && Input.GetKeyDown(KeyCode.E))
        {
            _canCollect = false; // Impede de iniciar de novo
            StartCoroutine(CollectTime());
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            _canCollect = false;
            Debug.Log("Left the item area.");
        }
    }

    IEnumerator CollectTime()
    {
        Debug.Log("Collecting item...");
        progressBar.gameObject.SetActive(true); // Mostra a barra de progresso
        progressBar.value = 0; // Garante que a barra comece vazia

        StartCoroutine(SkillCheckRoutine());// Lança a coroutine do skill check para rodar em paralelo.

        _elapsedTime = 0f;

        while (_elapsedTime < _collectTime)
        {
            // Se o jogador se mover, o MoveInput será diferente de zero.
            if (_playerInRange != null && _playerInRange.MoveInput != Vector2.zero)
            {
                Debug.Log("Collection Canceled: Player moved.");
                progressBar.gameObject.SetActive(false); // Esconde a barra
                skillCheckGroup.SetActive(false); // Garante que a skillcheck barra suma também
                _canCollect = true; // Permite tentar de novo
                yield break; // PARA a coroutine imediatamente!
            }

            _elapsedTime += Time.deltaTime;// Incrementa o tempo
            progressBar.value = Mathf.Clamp01(_elapsedTime / _collectTime); // Atualiza a barra de progresso
            yield return null;
        }

        // Se o loop terminar, a coleta foi um sucesso!
        progressBar.gameObject.SetActive(false); // Esconde a barra de progresso
        skillCheckGroup.SetActive(false);// Esconde a barra de skillcheck se estiver visível
        Debug.Log("Item Collected!");
        Destroy(gameObject); // Remove the item from the scene
    }

    IEnumerator SkillCheckRoutine()
    {
        Debug.Log("Skill check started!");
        skillCheckGroup.SetActive(true); // Mostra a UI do skill check

        float timer = 0f;
        bool skillCheckUsed = false;

        // O loop principal do skill check
        while (timer < skillCheckDuration && !skillCheckUsed)
        {
            // Faz o marcador da barra ir e voltar de 0 a 1
            skillCheckBar.value = Mathf.PingPong(Time.time * 2, 1);

            // Checa se o jogador apertou a tecla
            if (Input.GetKeyDown(skillCheckKey))
            {
                skillCheckUsed = true; // Impede de tentar de novo

                // Checa se o valor do marcador está dentro da zona de acerto que lemos da imagem
                if (skillCheckBar.value >= successMinValue && skillCheckBar.value <= successMaxValue)
                {
                    Debug.Log("SKILL CHECK SUCESSO!");
                    _elapsedTime += bonusTime; // Aplica a recompensa de tempo!
                }
                else
                {
                    Debug.Log("SKILL CHECK FALHA!");
                }
            }

            timer += Time.deltaTime;
            yield return null; // Espera o próximo frame
        }

        // Se o tempo acabar ou o jogador já tiver interagido, esconde a UI
        Debug.Log("Skill check finished.");
        skillCheckGroup.SetActive(false);
    }
}


