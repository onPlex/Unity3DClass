using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    public WeaponStatsList swordGroup;

    void Start()
    {
        foreach (var weapon in swordGroup.weapons)
        {
            Debug.Log($"{weapon.weaponName}: {weapon.damage} Damage, {weapon.range} Range");
        }
    }
}
