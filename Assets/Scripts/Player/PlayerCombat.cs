using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    // O PULO DO GATO:
    // A variável é do tipo "WeaponBase", não "BowWeapon".
    // Isso significa que ela aceita QUALQUER filho (Espada, Arco, Machado).
    public WeaponBase currentWeapon;

    private void Update()
    {
        // Se apertar Espaço e tiver uma arma na mão...
        if (Input.GetKeyDown(KeyCode.Space) && currentWeapon != null)
        {
            // ATACA! (O polimorfismo decide qual Attack() rodar)
            currentWeapon.Attack();
        }
    }
}