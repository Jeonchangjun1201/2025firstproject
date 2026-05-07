using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 아이스 보스 전용 체력 및 시각 효과 관리 스크립트 (죽음시 모든 애니메이션 정지, IsDead true)
/// </summary>
public class IceBossHealth : MonoBehaviour
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

    private bool isPhase2 = false; // 페이즈 전환 체크

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

        // 2. 죽음 체크는 그 다음에!
        if (currentHP <= 0)
            Die();
    }

    private void Update()
    {
        if (currentHP <= 100f && !isPhase2)
        {
            isPhase2 = true;
            Debug.Log("변신");
            if (spriteRenderer != null)
                spriteRenderer.color = new Color(0.5f, 0.5f, 0.5f); // 어둡게
        }
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

    /// <summary>
    /// 외부에서 페이즈2 여부를 확인할 수 있도록 제공
    /// </summary>
    public bool IsPhase2()
    {
        return isPhase2;
    }

    private IEnumerator HitFlash()
    {
        if (isDead) yield break;

        // 아이스 보스 전용: 파란빛 피격 효과
        if (spriteRenderer != null)
            spriteRenderer.color = new Color(0.5f, 0.7f, 1f, 1f);

        yield return new WaitForSeconds(0.1f);

        // 페이즈 색상 유지
        if (isPhase2 && spriteRenderer != null)
            spriteRenderer.color = Color.blue;
        else if (spriteRenderer != null)
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
            animator.SetBool("IsDead", true); // 죽음 애니메이션 즉시 전이
            animator.SetBool("IsMoving", false);
            animator.ResetTrigger("Attack1");
            animator.ResetTrigger("Attack2");
            spriteRenderer.color = Color.blue;

        // 시간 보상
        if (gameManager != null)
            gameManager.TimePlus(timeReward);

        if (OnBossDead != null)
            OnBossDead.Invoke();

        StartCoroutine(FadeOutAndDie());
    }

    private IEnumerator FadeOutAndDie()
    {
        float elapsed = 0f;
        Color color = isPhase2 ? new Color(0.5f, 0.5f, 0.5f) : originalColor;

        // 서서히 투명해지며 사라짐
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
