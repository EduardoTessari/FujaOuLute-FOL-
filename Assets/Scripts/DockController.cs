using UnityEngine;

public class DockController : MonoBehaviour
{
    [Header("Conexões")]
    // 1. A "ponte" para a JANELA (o script DepositUI.cs)
    public DockUI depositPanel;

    // 2. A "ponte" para o CÉREBRO (o script BoatProgress.cs)
    public BoatProgress boatProgressManager;

    // Variáveis privadas
    private Inventory _playerInventory;
    private bool _canDeposit = false;
    [SerializeField] private GameObject _btnToPress; //variavel para a ajuda visual do botao de coleta

    // --- OnTriggerEnter e Exit estão perfeitos, não mude nada ---

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            // Pega o INVENTÁRIO em vez do CONTROLE
            _playerInventory = collision.GetComponent<Inventory>();

            if (_btnToPress != null)
            {
                _btnToPress.SetActive(true);
            }
            
            _canDeposit = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            _playerInventory = null;

            if (_btnToPress != null)
            {
                _btnToPress.SetActive(false); // Desativa o botao de auxilio ao abrir a janela
            }

            _canDeposit = false;
        }
    }

    // --- A LÓGICA CORRIGIDA NO UPDATE ---

    private void Update()
    {
        // Se o jogador está na área e aperta a tecla
        if (_playerInventory != null && _canDeposit && Input.GetKeyDown(KeyCode.E))
        {
            // 3. A MÁGICA! A Doca "terceiriza" o trabalho.
            // Ela chama a função OpenWindow e entrega as "ferramentas"
            depositPanel.OpenWindow(_playerInventory, boatProgressManager);

            if (_btnToPress != null)
            {
                _btnToPress.SetActive(false); // Desativa o botao de auxilio ao abrir a janela
            }
                
        }
    }
}