using UnityEngine;
using UnityEngine.UI;

public class HPUI : MonoBehaviour
{
    public PlayerHP playerHP;
    public Slider hpBar;

    void Update()
    {
        hpBar.value = (float)playerHP.currentHP / playerHP.maxHP;
    }
}

