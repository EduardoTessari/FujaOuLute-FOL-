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

    // --- OnTriggerEnter e Exit estão perfeitos, não mude nada ---

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            // Pega o INVENTÁRIO em vez do CONTROLE
            _playerInventory = collision.GetComponent<Inventory>();
            _canDeposit = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            _playerInventory = null;
            _canDeposit = false;
        }
    }

    // --- A LÓGICA CORRIGIDA NO UPDATE ---

    private void Update()
    {
        // Se o jogador está na área e aperta a tecla
        if (_playerInventory != null && _canDeposit && Input.GetKeyDown(KeyCode.E))
        {
            // 3. Checamos se o inventário NÃO está vazio
            if (_playerInventory.GetCurrentInventory().Count > 0)
            {
                // 4. A MÁGICA! A Doca "terceiriza" o trabalho.
                // Ela chama a função OpenWindow e entrega as "ferramentas"
                depositPanel.OpenWindow(_playerInventory, boatProgressManager);
            }
            else
            {
                // Opcional: Tocar um som de "erro" (inventário vazio)
                Debug.Log("Inventário vazio!");
            }
        }
    }
}