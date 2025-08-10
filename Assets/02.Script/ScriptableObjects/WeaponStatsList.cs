using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "NewWeaponGroup", menuName = "ScriptableObjects/GameData/WeaponStatsList", order = 3)]
public class WeaponStatsList : ScriptableObject
{
    // 여러 개의 무기 데이터를 한 번에 관리
    public List<WeaponStats> weapons = new List<WeaponStats>();
}

