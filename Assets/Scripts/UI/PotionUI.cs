using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PotionUI : MonoBehaviour
{
    private TextMeshProUGUI potionText;

    private void Awake()
    {
        potionText = GetComponent<TextMeshProUGUI>();
    }
    
    public void UpdatePotionText(int count)
    {
        potionText.text = ": " + count;
    }
    public void UpdatePotionText(int count,Color color)
    {
        StartCoroutine(Wait(count,color));
    }

    private IEnumerator Wait(int count,Color color)
    {
        potionText.color = color;
        yield return new WaitForSeconds(0.2f);
        potionText.text = ": " + count;
        potionText.color = Color.white;
    }
}