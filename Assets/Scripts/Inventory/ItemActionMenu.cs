using UnityEngine;
using UnityEngine.UI;
using TMPro; // Adicionei caso use TextMeshPro nos botões

public class ItemActionMenu : MonoBehaviour
{
    public static ItemActionMenu Instance; // Singleton pra facilitar chamar ele de qualquer lugar

    [Header("UI References")]
    [SerializeField] GameObject menuPanel; // O Painel inteiro
    [SerializeField] Button btnEquip;      // Botão Equipar
    [SerializeField] Button btnCancel;     // Botão Cancelar

    private ItemData _currentItem; // Guarda qual item clicamos

    private void Awake()
    {
        // Configura o Singleton
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }

        // Já deixa os clicks configurados
        btnEquip.onClick.AddListener(OnEquipClicked);
        btnCancel.onClick.AddListener(CloseMenu);

        // Garante que começa fechado
        menuPanel.SetActive(false);
    }

    // Chamado pelo Slot quando clicamos no item
    public void OpenMenu(ItemData item, Vector2 position)
    {
        _currentItem = item;

        // Move o painel para perto do mouse
        // Nota: Dependendo do seu Canvas (Screen Space Overlay ou Camera), 
        // talvez precise de um ajuste fino aqui. Vamos testar assim primeiro.
        menuPanel.transform.position = position;

        menuPanel.SetActive(true);
    }

    public void CloseMenu()
    {
        menuPanel.SetActive(false);
        _currentItem = null;
    }

    void OnEquipClicked()
    {
        if (_currentItem != null)
        {
            // Busca o script no Player
            ChangeWeapon changeScript = FindAnyObjectByType<ChangeWeapon>();

            if (changeScript != null)
            {
                // CHAMA O MÉTODO NOVO PASSANDO O ITEM
                changeScript.EquipWeaponFromInventory(_currentItem);
                CloseMenu();
            }
            else
            {
                Debug.LogError("Não achei o script ChangeWeapon na cena!");
            }
        }
    }
}