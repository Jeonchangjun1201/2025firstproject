using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Enemy : MonoBehaviour
{
    [SerializeField] float currentHP;
    public float maxHP = 100f;

    [Header("HP UI")]
    public Image hpGauge; // 월드 스페이스 Canvas의 Filled Image
    public TextMeshProUGUI hpText; // 월드 스페이스 Canvas의 TMP Text

    private SpriteRenderer spriteRenderer;
    private ParticleSystem particleSystem;
    private Color originalColor;
    public bool isDead = false;

    public GameObject backGround;
    public GameObject damageTextPrefab;
    private Animator animator;
    public GameObject player;
    public GameObject gameManager;
    private GameManager1 gameManager1;
    private int time = 10;
    
    void Awake()
    {
        currentHP = maxHP;
        spriteRenderer = GetComponent<SpriteRenderer>();
        particleSystem = GetComponent<ParticleSystem>();
        animator = GetComponent<Animator>();
        particleSystem.Stop();
        if (spriteRenderer == null)
        {
            Debug.LogError("SpriteRenderer가 없습니다!");
            enabled = false;
            return;
        }

        originalColor = spriteRenderer.color;
        UpdateHpUI();
        gameManager1 = gameManager.GetComponent<GameManager1>();
    }


    public void TakeDamage(float damage)
    {
        if (isDead) return;
        currentHP -= damage;
        currentHP = Mathf.Max(currentHP, 0f);
        UpdateHpUI();
        StartCoroutine(HitFlash());
        ShowDamageText(damage);

        if (currentHP <= 0)
        {
            Die();
        }
    }

    private IEnumerator HitFlash()
    {
        if (isDead) yield break;
        spriteRenderer.color = new Color(1f, 0.3f, 0.3f, 0.7f);
        particleSystem.Play();
        yield return new WaitForSeconds(0.05f);
        spriteRenderer.color = originalColor;
        yield return new WaitForSeconds(0.125f);
        particleSystem.Stop();
    }

    private void ShowDamageText(float damage)
    {
        if (isDead) return;
        if (damageTextPrefab == null)
        {
            Debug.Log("Prefab is null");
            return;
        }

        GameObject textObj = Instantiate(damageTextPrefab, transform.position, Quaternion.identity);
        DamageText text = textObj.GetComponent<DamageText>();
        if (text != null)
        {
            text.SetText(damage.ToString("0"));
        }
        else
        {
            Debug.LogWarning("DamageText 컴포넌트가 없습니다!");
        }
    }

    private void Die()
    {
        isDead = true;
        StopAllCoroutines();
        gameObject.layer = LayerMask.NameToLayer("Dead");
        animator.SetTrigger("Dead");
        StartCoroutine(TimeAdd());
        spriteRenderer.color = originalColor;
        UpdateHpUI(); // 죽을 때도 HP UI 0으로 갱신
        StartCoroutine(FadeOutAndDie(1.5f));
    }

    private IEnumerator TimeAdd()
    {
        yield return new WaitForSeconds(0.5f);
        gameManager1.TimePlus(time);
    }

    private IEnumerator FadeOutAndDie(float duration)
    {
        float elapsed = 0f;
        Color color = originalColor;

        yield return new WaitForSeconds(0.1f);
        particleSystem.Stop();

        while (elapsed < duration)
        {
            float alpha = Mathf.Lerp(1f, 0f, elapsed / duration);
            spriteRenderer.color = new Color(color.r, color.g, color.b, alpha);
            elapsed += Time.deltaTime;
            yield return null;
        }

        spriteRenderer.color = new Color(color.r, color.g, color.b, 0f);
        Destroy(hpGauge);
        Destroy(hpText);
        Destroy(backGround);
        Destroy(gameObject);
    }

    // HP UI 갱신
    private void UpdateHpUI()
    {
        if (hpGauge != null)
            hpGauge.fillAmount = Mathf.Max(currentHP, 0) / maxHP;
        if (hpText != null)
            hpText.text = $"{Mathf.Max((int)currentHP, 0)}/{(int)maxHP}";
    }
}
