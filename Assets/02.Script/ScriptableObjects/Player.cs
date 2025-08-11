using UnityEngine;

/// <summary>
/// 플레이어의 기본 동작을 담당하는 컴포넌트
/// - 체력 관리 및 데미지 처리
/// </summary>
public class Player : MonoBehaviour
{
    public PlayerHP playerHP;  // 플레이어 체력 데이터

    /// <summary>
    /// 플레이어가 데미지를 받았을 때 호출되는 함수
    /// </summary>
    /// <param name="amount">받을 데미지 양</param>
    public void TakeDamage(int amount)
    {
        playerHP.currentHP -= amount;
        playerHP.currentHP = Mathf.Max(playerHP.currentHP, 0); // 0 이하로 내려가지 않도록 제한
    }
}

