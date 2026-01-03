using Assets.HeroEditor4D.Common.Scripts.Common;
using Unity.VisualScripting;
using UnityEngine;

public class ChangeWeapon : MonoBehaviour
{
    [Header("Referências Visuais")]
    [SerializeField] GameObject _weaponPosition; // A Mão (WL)
    [SerializeField] GameObject[] newWeapon; // Seus Prefabs (Arco, Espada)
    [SerializeField] ItemData weaponData; // Dados da arma para o inventário

    [Header("Referência Lógica")]
    // --- NOVO: Precisamos saber QUEM é o Player para avisar ele ---
    [SerializeField] PlayerCombat _playerCombat;

    // (Não precisa de Awake ou bool _hasWeapon por enquanto)

    public void AddToInventory()
    {
               // Adiciona a arma ao inventário
        InventoryUI inventory = FindAnyObjectByType<InventoryUI>();
        if (inventory != null && newWeapon[0] != null)
        {
            if (weaponData != null)
            {
                inventory.AddItem(weaponData, 1);
                Debug.Log($"Arma {weaponData.itemName} adicionada ao inventário.");
            }
            else
            {
                Debug.LogError("ERRO: O prefab da arma não tem WeaponPickup ou ItemData!");
            }
        }
        else
        {
            Debug.LogError("ERRO: InventoryUI não encontrado ou nova arma não atribuída!");
        }
    }

    public void EquipWeaponFromInventory(ItemData itemRecebido)
    {
        // Validação básica
        if (itemRecebido == null || itemRecebido.prefab == null)
        {
            Debug.LogError("Tentou equipar item inválido ou sem prefab!");
            return;
        }

        // 1. LIMPEZA (Mesma lógica)
        if (_weaponPosition.transform.childCount > 0)
        {
            foreach (Transform child in _weaponPosition.transform) Destroy(child.gameObject);
        }

        // 2. NASCIMENTO (Usa o itemRecebido como fonte)
        GameObject novaArmaObj = Instantiate(itemRecebido.prefab, _weaponPosition.transform.position, _weaponPosition.transform.rotation, _weaponPosition.transform);

        novaArmaObj.transform.localPosition = Vector3.zero;
        novaArmaObj.transform.localRotation = Quaternion.identity;

        // 3. CASAMENTO
        WeaponBase scriptDaArma = novaArmaObj.GetComponent<WeaponBase>();

        if (_playerCombat != null && scriptDaArma != null)
        {
            _playerCombat.currentWeapon = scriptDaArma;
            Debug.Log($"[INVENTÁRIO] Equipado: {scriptDaArma.weaponName}");
        }
    }
}