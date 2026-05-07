using UnityEngine;

public class PotionInventory : MonoBehaviour
{
    public static PotionInventory Instance;
    public int potionCount = 0;

    private void Awake()
    {
        Instance = this;
        potionCount = 0;
        FindObjectOfType<PotionUI>().UpdatePotionText(potionCount);
    }

    public void AddPotion(int amount)
    {
        potionCount += amount;
        FindObjectOfType<PotionUI>().UpdatePotionText(potionCount,Color.green);
    }

    public bool UsePotion()
    {
        if (potionCount > 0)
        {
            potionCount--;
            FindObjectOfType<PotionUI>().UpdatePotionText(potionCount,Color.red);
            return true;
        }
        return false;
    }
}