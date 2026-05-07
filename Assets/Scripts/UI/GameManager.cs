using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public int coinCount = 0;
    public TextMeshProUGUI coinText;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void AddCoin(int amount)
    {
        coinCount += amount;
        UpdateCoinUI();
    }

    public bool TryUseCoin(int amount)
    {
        if (coinCount >= amount)
        {
            coinCount -= amount;
            UpdateCoinUI();
            return true;
        }
        return false;
    }

    void UpdateCoinUI()
    {
        if (coinText != null)
            coinText.text = $": {coinCount}";
    }
}