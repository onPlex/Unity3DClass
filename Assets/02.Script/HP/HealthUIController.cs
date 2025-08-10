using UnityEngine;
using UnityEngine.UIElements;
public class HealthUIController : MonoBehaviour
{
    public PlayerHP playerHP; // ScriptableObject 참조
    private ProgressBar hpBar;

    private void OnEnable()
    {
        // UI Document 가져오기
        var root = GetComponent<UIDocument>().rootVisualElement;

        // UXML에서 name="hp-bar"로 지정한 ProgressBar 찾기
        hpBar = root.Q<ProgressBar>("hp-bar");
    }

    private void Update()
    {
        // 체력 갱신 (실시간 반영)
        hpBar.value = playerHP.currentHP;
        hpBar.highValue = playerHP.maxHP;
    }
}
