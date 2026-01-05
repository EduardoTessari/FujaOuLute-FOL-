using Assets.HeroEditor4D.Common.Scripts.ExampleScripts;
using UnityEngine;

public class BowWeapon : WeaponBase // Herda do Pai!
{
    [Header("Configurações do Arco")]
    // (Removemos attackCD e attackKey. O Pai cuida do tempo, o Player cuida da tecla)

    [SerializeField] private GameObject _projectile; // O Prefab da Flecha
    [SerializeField] private Transform _firePoint;   // Mudei para TRANSFORM (mais fácil)

    // O Override é a única coisa que precisamos!
    public override void Attack()
    {
        // 1. Checa o cooldown usando a variável 'cooldown' do Pai (WeaponBase)
        if (!CanAttack()) return;

        // 2. Roda a lógica do Pai (reseta o timer lastAttackTime)
        base.Attack();

        // --- A SUA LÓGICA DE TIRO ---

        // 3. Instancia (Usando _firePoint.position direto pq agora é Transform)
        GameObject arrowObj = Instantiate(_projectile, _firePoint.position, Quaternion.identity);

        // 4. Calcula a Direção (Baseado na escala do Player/Pai)
        Vector2 shootDirection;
        if (transform.lossyScale.x < 0) shootDirection = Vector2.left;
        else shootDirection = Vector2.right;

        // 5. Lança a flecha (Pega o script Projectile que criamos)
        ProjectileMovement arrowScript = arrowObj.GetComponent<ProjectileMovement>();
        if (arrowScript != null) arrowScript.Launch(shootDirection);

        // 6. Configura o Dano (Pega o 'damage' do Pai WeaponBase)
        DamageDealer dealer = arrowObj.GetComponent<DamageDealer>();
        if (dealer != null) dealer.SetDamage(damage);
    }
}