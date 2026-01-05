using UnityEngine;

public class HealingStaff : WeaponBase
{
    [Header("Configurações do Mago")]
    [SerializeField] private GameObject healingZonePrefab; // O círculo verde

    public override void Attack()
    {
        // 1. Respeita o cooldown do pai
        if (!CanAttack()) return;
        base.Attack();

        // 2. A Lógica Única:
        // Instancia no PÉ do jogador (transform.position), sem rotação.
        Instantiate(healingZonePrefab, transform.position, Quaternion.identity);

        Debug.Log("CURA LANÇADA!");
    }
}