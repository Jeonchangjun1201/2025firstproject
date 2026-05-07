using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WIndBossHealth : MonoBehaviour
{
    
    public GameObject Ground;
    public GameObject Reward;
    [Header("HP Settings")]
    public float maxHP = 200f;
    public float currentHP;
    public bool isDead = false;

    [Header("HP UI")]
    public Image hpGauge;
    public Image hpGaugeBackground;
    public TextMeshProUGUI hpText;
    public GameObject damageTextPrefab;
    public GameObject HpBarTransform;

    [Header("Visual Effects")]
    public float deathFadeDuration = 1.5f;
    public int timeReward = 60; // 처치 시 시간 보상 (60초)
    private SpriteRenderer spriteRenderer;
    private Animator animator;
    private GameManager1 gameManager;
    private Color originalColor;

    // === 보스 사망시 알림용 이벤트 ===
    public event Action OnBossDead;

    void Awake()
    {
        currentHP = maxHP;
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();

        if (spriteRenderer != null)
            originalColor = spriteRenderer.color;

        UpdateHpUI();
    }

    void Start()
    {
        gameManager = GameObject.FindWithTag("GameManager1").GetComponent<GameManager1>();
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
            Die();
    }

    private void Update()
    {
        if (HpBarTransform != null)
        {
            if (hpGauge != null)
                hpGauge.transform.position = HpBarTransform.transform.position;
            if (hpGaugeBackground != null)
                hpGaugeBackground.transform.position = HpBarTransform.transform.position;
            if (hpText != null)
                hpText.transform.position = HpBarTransform.transform.position;
        }
    }

    private IEnumerator HitFlash()
    {
        if (isDead) yield break;

        // 피격 효과
        if (spriteRenderer != null)
            spriteRenderer.color = Color.red;

        yield return new WaitForSeconds(0.1f);

        if (spriteRenderer != null)
            spriteRenderer.color = originalColor;
    }

    private void ShowDamageText(float damage)
    {
        if (damageTextPrefab == null) return;

        GameObject textObj = Instantiate(damageTextPrefab, transform.position, Quaternion.identity);
        DamageText text = textObj.GetComponent<DamageText>();
        if (text != null) text.SetText(damage.ToString("0"));
    }

    private void Die()
    {
        isDead = true;
        if (hpGauge != null) Destroy(hpGauge.gameObject);
        if (hpText != null) Destroy(hpText.gameObject);
        if (hpGaugeBackground != null) Destroy(hpGaugeBackground.gameObject);
        Debug.Log("죽어");
        if (animator != null)
        {
            animator.SetBool("IsDead", true);
            animator.SetBool("IsMoving", false);
            animator.ResetTrigger("Attack1");
            animator.ResetTrigger("Attack2");
        }
        if (spriteRenderer != null)
            spriteRenderer.color = Color.blue;

        if (gameManager != null)
            gameManager.TimePlus(timeReward);

        if (OnBossDead != null)
            OnBossDead.Invoke();

        StartCoroutine(FadeOutAndDie());
    }

    private IEnumerator FadeOutAndDie()
    {
        float elapsed = 0f;
        Color color = originalColor;

        while (elapsed < deathFadeDuration)
        {
            float alpha = Mathf.Lerp(1f, 0f, elapsed / deathFadeDuration);
            if (spriteRenderer != null)
                spriteRenderer.color = new Color(color.r, color.g, color.b, alpha);
            elapsed += Time.deltaTime;
            yield return null;
        }
        Reward.SetActive(true);
        Ground.SetActive(true);
        Destroy(gameObject);
        Destroy(hpGauge);
        Destroy(hpText);
    }

    private void UpdateHpUI()
    {
        if (hpGauge != null)
            hpGauge.fillAmount = Mathf.Max(currentHP, 0) / maxHP;

        if (hpText != null)
            hpText.text = $"{Mathf.Max((int)currentHP, 0)}/{(int)maxHP}";
    }
}
