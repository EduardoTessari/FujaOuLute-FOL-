using UnityEngine;

public class DockController : MonoBehaviour
{
    private PLayerControler _playerInRange = null;
    private bool _canDeposit = false;

    private void Update()
    {
        if (_playerInRange != null && _canDeposit && Input.GetKeyDown(KeyCode.E))
        {
            _playerInRange.GetComponent<Inventory>().DepositWood(1);
            // Aqui você pode adicionar a lógica para atualizar o inventário do jogador, etc.
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            _playerInRange = collision.GetComponent<PLayerControler>();
            _canDeposit = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            _playerInRange = null;
            _canDeposit = false;
        }
    }
}
