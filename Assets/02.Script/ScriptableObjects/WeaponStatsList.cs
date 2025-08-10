using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "NewWeaponGroup", menuName = "ScriptableObjects/GameData/WeaponStatsList", order = 3)]
public class WeaponStatsList : ScriptableObject
{
    // ���� ���� ���� �����͸� �� ���� ����
    public List<WeaponStats> weapons = new List<WeaponStats>();
}

