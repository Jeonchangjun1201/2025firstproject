using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ShopManager : MonoBehaviour
{
    public Button healthPotionButton; // 인스펙터에서 버튼 할당
    private Color originalColor;

    void Start()
    {
        if (healthPotionButton != null)
        {
            originalColor = healthPotionButton.image.color;
        }
    }

    public void BuyHealthPotion()
    {
        if (GameManager.Instance.TryUseCoin(30))
        {
            // 포션 구매 성공
            PotionInventory.Instance.AddPotion(1);
            Debug.Log("포션 구매 성공");
        }
        else
        {
            // 코인 부족 - 버튼 색상 변경
            StartCoroutine(FlashButton());
            Debug.Log("코인이 부족합니다!");
        }
    }

    private IEnumerator FlashButton()
    {
        if (healthPotionButton != null)
        {
            // 빨간색으로 변경
            healthPotionButton.image.color = Color.red;
            
            // 0.5초 대기
            yield return new WaitForSeconds(0.5f);
            
            // 원래 색상으로 복원
            healthPotionButton.image.color = originalColor;
        }
    }
}