using Assets.HeroEditor4D.Common.Scripts.Common;
using Unity.VisualScripting;
using UnityEngine;

public class ChangeWeapon : MonoBehaviour
{
    [SerializeField] GameObject _weaponPosition; // Referência ao objeto da mão da arma

    [SerializeField]GameObject[] newWeapon;
    bool _hasWeapon = false;

    private void Awake()
    {
        //_hasWeapon = true;
    }

    public void ChangeWeaponCondicion()
    {

        //if (_hasWeapon)
        //{
            Instantiate(newWeapon[0], _weaponPosition.transform.position, newWeapon[0].transform.rotation, _weaponPosition.transform);
        //}
        
        

    }
}
