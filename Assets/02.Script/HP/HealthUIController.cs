using UnityEngine;
using UnityEngine.UIElements;
public class HealthUIController : MonoBehaviour
{
    public PlayerHP playerHP; // ScriptableObject ����
    private ProgressBar hpBar;

    private void OnEnable()
    {
        // UI Document ��������
        var root = GetComponent<UIDocument>().rootVisualElement;

        // UXML���� name="hp-bar"�� ������ ProgressBar ã��
        hpBar = root.Q<ProgressBar>("hp-bar");
    }

    private void Update()
    {
        // ü�� ���� (�ǽð� �ݿ�)
        hpBar.value = playerHP.currentHP;
        hpBar.highValue = playerHP.maxHP;
    }
}
