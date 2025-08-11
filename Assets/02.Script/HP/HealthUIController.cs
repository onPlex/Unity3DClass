using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
/// 체력 UI를 제어하는 컨트롤러
/// - UXML 기반의 UI Toolkit을 사용하여 체력바를 업데이트
/// </summary>
public class HealthUIController : MonoBehaviour
{
    public PlayerHP playerHP; // ScriptableObject 참조
    private ProgressBar hpBar;

    private void OnEnable()
    {
        // UI Document 루트 요소 가져오기
        var root = GetComponent<UIDocument>().rootVisualElement;

        // UXML에서 name="hp-bar"인 ProgressBar 찾기
        hpBar = root.Q<ProgressBar>("hp-bar");
    }

    private void Update()
    {
        // 체력 업데이트 (실시간 반영)
        hpBar.value = playerHP.currentHP;
        hpBar.highValue = playerHP.maxHP;
    }
}
