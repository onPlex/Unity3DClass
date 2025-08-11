using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 무기 스탯 데이터를 그룹으로 관리하는 ScriptableObject
/// - 여러 무기의 스탯 정보를 하나의 에셋에 저장
/// </summary>
[CreateAssetMenu(fileName = "NewWeaponGroup", menuName = "ScriptableObjects/GameData/WeaponStatsList", order = 3)]
public class WeaponStatsList : ScriptableObject
{
    // 여러 무기 스탯 데이터를 담는 리스트
    public List<WeaponStats> weapons = new List<WeaponStats>();
}

