using UnityEngine;

public abstract class WeaponBase : MonoBehaviour
{
    [Header("Status da Arma")]
    public string weaponName;
    public int damage = 1;
    public float cooldown = 0.5f; // Tempo entre ataques

    protected float lastAttackTime; // "protected" para os filhos verem

    // "virtual" = Os filhos podem mudar o que isso faz!
    public virtual void Attack()
    {
        lastAttackTime = Time.time; // Reseta o cooldown
        // (Aqui pode ir um som genérico de ataque no futuro, se quiser)
    }

    // Função auxiliar para saber se já pode atacar
    public bool CanAttack()
    {
        return Time.time >= lastAttackTime + cooldown;
    }
}