using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 전통적인 UI 시스템을 사용하는 체력 UI
/// - Slider 컴포넌트를 사용하여 체력바 표시
/// </summary>
public class HPUI : MonoBehaviour
{
    public PlayerHP playerHP;  // 플레이어 체력 데이터
    public Slider hpBar;       // 체력바 UI 요소

    void Update()
    {
        // 현재 체력을 최대 체력으로 나누어 0~1 사이의 값으로 정규화
        hpBar.value = (float)playerHP.currentHP / playerHP.maxHP;
    }
}

