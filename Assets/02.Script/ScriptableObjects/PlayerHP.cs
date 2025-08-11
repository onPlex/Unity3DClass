using UnityEngine;

/// <summary>
/// 플레이어의 체력 정보를 저장하는 ScriptableObject
/// - 현재 체력과 최대 체력을 관리
/// - 여러 씬에서 동일한 체력 데이터 공유 가능
/// </summary>
[CreateAssetMenu(fileName = "PlayerHP", menuName = "ScriptableObjects/GameData/PlayerHP", order = 1)]
public class PlayerHP : ScriptableObject
{
    public int currentHP = 100;  // 현재 체력
    public int maxHP = 100;      // 최대 체력
}

