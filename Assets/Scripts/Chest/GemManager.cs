using UnityEngine;

public class GemManager : MonoBehaviour
{
    public static GemManager Instance;

    public bool hasEndingGem = false;
    public bool hasGem1 = false;
    public bool hasGem2 = false;
    public bool hasGem3 = false;

    private void Awake()
    {
        // 싱글톤 패턴: Instance 초기화
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject); // 중복 방지
        }
    }

    public bool HasAllGems()
    {
        return hasEndingGem && hasGem1 && hasGem2 && hasGem3;
    }
}