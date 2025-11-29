using System.Collections; // Necessário para Corrotinas
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [Header("Configuração do Ataque")]
    [SerializeField] private float attackCD = 3f; // Quanto tempo pra criar um novo projetil"
    [SerializeField] private KeyCode attackKey = KeyCode.Space; // Botão de ataque
    [SerializeField] private GameObject _projectile; // Hitbox da espada
    [SerializeField] private GameObject _firePoint; // Ponto de onde sai o projetil

    [Header("Estado")]
    private bool isAttacking = false;

    private void Update()
    {
        // Se apertar Espaço E não estiver atacando agora
        if (Input.GetKeyDown(attackKey) && !isAttacking)
        {
            StartCoroutine(PerformAttack());
        }
    }

    private IEnumerator PerformAttack()
    {
        isAttacking = true;

        GameObject projectilGO = Instantiate(_projectile, _firePoint.transform.position, _firePoint.transform.rotation);

        // 2. Pega o script da flecha que acabamos de criar
        ProjectileMovement arrowScript = projectilGO.GetComponent<ProjectileMovement>();

        // 3. Descobre a direção baseada na Escala do Player (Esquerda ou Direita?)
        // lossyScale.x pega a escala global (considerando o pai)
        Vector2 shootDirection;

        if (transform.lossyScale.x < 0)
        {
            shootDirection = Vector2.left; // (-1, 0)
        }
        else
        {
            shootDirection = Vector2.right; // (1, 0)
        }

        if (arrowScript != null)
        {
            arrowScript.Launch(shootDirection);
        }

        yield return new WaitForSeconds(attackCD);

        isAttacking = false;
    }
}
