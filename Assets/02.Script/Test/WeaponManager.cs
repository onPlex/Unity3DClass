using UnityEngine;

/// <summary>
/// 무기 데이터를 테스트하고 관리하는 매니저
/// - ScriptableObject에서 무기 정보를 읽어와서 콘솔에 출력
/// </summary>
public class WeaponManager : MonoBehaviour
{
    public WeaponStatsList swordGroup;  // 무기 스탯 데이터 그룹

    void Start()
    {
        // 모든 무기의 정보를 콘솔에 출력
        foreach (var weapon in swordGroup.weapons)
        {
            Debug.Log($"{weapon.weaponName}: {weapon.damage} Damage, {weapon.range} Range");
        }
    }
}
