using System.Collections;
using UnityEngine;

public class CollectItem : MonoBehaviour
{
    [Header("Configuração da Coleta")]
    [SerializeField] float _collectTime = 10f;
    [SerializeField] float _bonusTime = 6f;

    [Header("Configuração do Item")]
    public ItemList itemToGive;
    public int amountToGive = 1;


    // Variáveis Privadas
    private bool _canCollect = false;
    private PLayerControler _playerInRange = null;
    private Coroutine _collectCoroutine;

    // --- CORREÇÃO DO BUG CS0103 ---
    // Promovemos o _elapsedTime para ser uma variável da classe
    private float _elapsedTime;
    // ----------------------------

    private void Awake()
    {
    }

    // --- CORREÇÃO DE LÓGICA NO TRIGGER EXIT ---
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

                // --- CORREÇÃO DO BUG DE ARQUITETURA ---
                // O "Chefe" (CollectItem) não deve desligar a UI
                // UIManager.instance.skillCheckGroup.SetActive(false); // DELETADO
                // Em vez disso, ele AVISA o UIManager
                UIManager.instance.HideSkillCheckUI();
                // ------------------------------------
            }
        }
    }
    // --- FIM DA CORREÇÃO ---

    // (OnTriggerEnter e Update continuam iguais)

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
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

        AudioManager.instance.PlayCollectingLoop(itemToGive);

        Coroutine skillCheckCoroutine = StartCoroutine(UIManager.instance.DoSkillCheckProcess(this));

        while (_elapsedTime < _collectTime)
        {
            if (_playerInRange != null && _playerInRange.MoveInput != Vector2.zero)
            {
                // Cancelamento por movimento
                UIManager.instance.ShowProgressBar(false);
                StopCoroutine(skillCheckCoroutine);

                // --- CORREÇÃO DO BUG DE ARQUITETURA ---
                UIManager.instance.HideSkillCheckUI();
                // ------------------------------------

                AudioManager.instance.StopCollectingLoop();
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
        UIManager.instance.HideSkillCheckUI(); // Garante que a UI suma

        _playerInRange.GetComponent<Inventory>().AddItem(itemToGive, amountToGive);
        Destroy(gameObject);
        _collectCoroutine = null;
    }

    // Esta função agora funciona, pois _elapsedTime é da classe!
    public void ApplySkillCheckBonus()
    {
        _elapsedTime += _bonusTime;
    }
}