using System.Collections;
using UnityEngine;

public class CollectItem : MonoBehaviour
{
    [Header("Configuração da Coleta")]
    [SerializeField] float _collectTime = 10f;
    [SerializeField] float _bonusTime = 6f;

    [Header("Configuração do Item")]
    // MUDANÇA AQUI: Agora usamos o ScriptableObject, não mais o Enum
    public ItemData itemToGive;
    public int amountToGive = 1;

    // Variáveis Privadas
    private bool _canCollect = false;
    private PLayerControler _playerInRange = null; // Verifique se é PLayerControler ou PlayerController no seu projeto
    private Coroutine _collectCoroutine;
    private float _elapsedTime;

    private void Awake()
    {
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            _playerInRange = null;
            UIManager.instance.ShowInteractPrompt(false);
            _canCollect = false;

            if (_collectCoroutine != null)
            {
                StopCoroutine(_collectCoroutine);
                UIManager.instance.ShowProgressBar(false);
                UIManager.instance.HideSkillCheckUI();
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            // Cuidado aqui: garanta que o nome do script é PLayerControler mesmo (com L maiúsculo)
            _playerInRange = collision.GetComponent<PLayerControler>();
            UIManager.instance.ShowInteractPrompt(true);
            _canCollect = true;
        }
    }

    private void Update()
    {
        if (_playerInRange != null && _canCollect && Input.GetKeyDown(KeyCode.E))
        {
            _canCollect = false;
            UIManager.instance.ShowInteractPrompt(false);
            _collectCoroutine = StartCoroutine(CollectTime());
        }
    }

    IEnumerator CollectTime()
    {
        UIManager.instance.ShowProgressBar(true);
        _elapsedTime = 0f;

        // --- ATENÇÃO: Comentei o áudio temporariamente para não dar erro ---
        // AudioManager.instance.PlayCollectingLoop(itemToGive); 
        // ------------------------------------------------------------------

        Coroutine skillCheckCoroutine = StartCoroutine(UIManager.instance.DoSkillCheckProcess(this));

        while (_elapsedTime < _collectTime)
        {
            if (_playerInRange != null && _playerInRange.MoveInput != Vector2.zero)
            {
                // Cancelamento por movimento
                UIManager.instance.ShowProgressBar(false);
                StopCoroutine(skillCheckCoroutine);
                UIManager.instance.HideSkillCheckUI();

                // AudioManager.instance.StopCollectingLoop(); // Comentado temporariamente
                _canCollect = true;
                _collectCoroutine = null;
                yield break;
            }

            _elapsedTime += Time.deltaTime;
            UIManager.instance.UpdateProgressBar(_elapsedTime / _collectTime);
            yield return null;
        }

        // SUCESSO!
        AudioManager.instance.PlayDepositSound(true);
        AudioManager.instance.StopCollectingLoop();  
        
        StopCoroutine(skillCheckCoroutine);
        UIManager.instance.ShowProgressBar(false);
        UIManager.instance.HideSkillCheckUI();

        // MUDANÇA CRUCIAL: Agora o AddItem recebe o ItemData corretamente!
        _playerInRange.GetComponent<InventoryUI>().AddItem(itemToGive, amountToGive);

        Destroy(gameObject);
        _collectCoroutine = null;
    }

    public void ApplySkillCheckBonus()
    {
        _elapsedTime += _bonusTime;
    }
}