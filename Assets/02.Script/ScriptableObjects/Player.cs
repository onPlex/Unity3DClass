using UnityEngine;

public class Player : MonoBehaviour
{
    public PlayerHP playerHP;

    public void TakeDamage(int amount)
    {
        playerHP.currentHP -= amount;
        playerHP.currentHP = Mathf.Max(playerHP.currentHP, 0); // 0 미만 방지
    }
}

