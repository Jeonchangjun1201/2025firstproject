using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BossHealth : MonoBehaviour
{
    
    public GameObject Ground;
    public GameObject Reward;
    [Header("HP Settings")]
    public float maxHP = 1000f;
    public float currentHP;
    public bool isDead = false;
    
    [Header("HP UI")]
    public Image hpGauge;
    public TextMeshProUGUI hpText;
    public Image hpGaugeBackground;
    public GameObject damageTextPrefab;
    
    [Header("Visual Effects")]
    public float deathFadeDuration = 1.5f;
    public int timeReward = 60; // 처치 시 시간 보상 (60초)
    
    private ParticleSystem hitParticles;
    private SpriteRenderer spriteRenderer;
    private Animator animator;
    private GameManager1 gameManager;
    private Color originalColor;

    void Awake()
    {
        hitParticles = GetComponent<ParticleSystem>();
        currentHP = maxHP;
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        
        if (spriteRenderer != null)
            originalColor = spriteRenderer.color;
        
        UpdateHpUI();
    }

    void Start()
    {
        gameManager = GameObject.FindWithTag("GameManager").GetComponent<GameManager1>();
    }

    public void TakeDamage(float damage)
    {
        if (isDead) return;
        
        currentHP -= damage;
        currentHP = Mathf.Max(currentHP, 0f);
        UpdateHpUI();
        
        // 피격 효과
        StartCoroutine(HitFlash());
        ShowDamageText(damage);
        
        if (currentHP <= 0)
            Die();
    }

    private IEnumerator HitFlash()
    {
        if (isDead) yield break;
        
        // 피격 시 빨간색 깜빡임 + 파티클
        spriteRenderer.color = new Color(1f, 0.3f, 0.3f, 0.7f);
        if (hitParticles != null) hitParticles.Play();
        
        yield return new WaitForSeconds(0.05f);
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
        gameObject.layer = LayerMask.NameToLayer("Dead");
        
        // 사망 애니메이션
        if (animator != null) 
            animator.SetBool("isDead",true);
        
        // 시간 보상
        if (gameManager != null) 
            gameManager.TimePlus(timeReward);
        
        // 페이드 아웃 및 제거
        StartCoroutine(FadeOutAndDie());
    }

    private IEnumerator FadeOutAndDie()
    {
        float elapsed = 0f;
        Color color = originalColor;
        
        // 파티클 정지
        if (hitParticles != null) 
            hitParticles.Stop();

        // 서서히 투명해지며 사라짐
        while (elapsed < deathFadeDuration)
        {
            float alpha = Mathf.Lerp(1f, 0f, elapsed / deathFadeDuration);
            spriteRenderer.color = new Color(color.r, color.g, color.b, alpha);
            elapsed += Time.deltaTime;
            yield return null;
        }
        Reward.SetActive(true);
        Ground.SetActive(true);
        Destroy(gameObject);
    }

    private void UpdateHpUI()
    {
        if (hpGauge != null)
            hpGauge.fillAmount = Mathf.Max(currentHP, 0) / maxHP;
        
        if (hpText != null)
            hpText.text = $"{Mathf.Max((int)currentHP, 0)}/{(int)maxHP}";
    }

}
