using UnityEngine;

public class PlayerPotionUse : MonoBehaviour
{
    public PlayerHealth playerHealth; // PlayerHealth 스크립트 연결

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            if (PotionInventory.Instance.UsePotion())
            {
                playerHealth.Heal(30); // 회복량은 필요에 따라 조정
                // 이펙트/사운드 추가 가능
            }
        }
    }
}