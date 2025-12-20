using Assets.HeroEditor4D.Common.Scripts.Common;
using Unity.VisualScripting;
using UnityEngine;

public class ChangeWeapon : MonoBehaviour
{
    [Header("Referências Visuais")]
    [SerializeField] GameObject _weaponPosition; // A Mão (WL)
    [SerializeField] GameObject[] newWeapon; // Seus Prefabs (Arco, Espada)

    [Header("Referência Lógica")]
    // --- NOVO: Precisamos saber QUEM é o Player para avisar ele ---
    [SerializeField] PlayerCombat _playerCombat;

    // (Não precisa de Awake ou bool _hasWeapon por enquanto)

    public void ChangeWeaponCondicion()
    {
      
        // 1. LIMPEZA (O Faxineiro)
        // Se já tem alguma coisa na mão, destrói tudo antes de criar a nova
        if (_weaponPosition.transform.childCount > 0)
        {
            foreach (Transform child in _weaponPosition.transform)
            {
                Destroy(child.gameObject);
            }
        }

        // 2. NASCIMENTO (O que você já tinha, mas simplificado)
        // Instancia como FILHO da mão direto.
        GameObject novaArmaObj = Instantiate(newWeapon[0], _weaponPosition.transform.position, newWeapon[0].transform.rotation, _weaponPosition.transform);

        // 3. O CASAMENTO (A Conexão Lógica)
        // Pega o script 'BowWeapon' (que é um WeaponBase) de dentro do objeto novo
        WeaponBase scriptDaArma = novaArmaObj.GetComponent<WeaponBase>();

        if (_playerCombat != null && scriptDaArma != null)
        {
            // Avisa o Player: "Toma, essa é sua nova arma atual"
            _playerCombat.currentWeapon = scriptDaArma;
            Debug.Log($"Arma trocada! Agora é: {scriptDaArma.weaponName}");
        }
        else
        {
            Debug.LogError("ERRO: Faltou arrastar o PlayerCombat no Inspector ou a Arma não tem script!");
        }
    }
}