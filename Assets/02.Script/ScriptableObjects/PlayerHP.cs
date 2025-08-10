using UnityEngine;

[CreateAssetMenu(fileName = "PlayerHP", menuName = "ScriptableObjects/GameData/PlayerHP", order = 1)]
public class PlayerHP : ScriptableObject
{
    public int currentHP = 100;
    public int maxHP = 100;
}

