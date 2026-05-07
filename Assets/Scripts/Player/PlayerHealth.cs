using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private int maxHp = 100;
    [SerializeField] int currentHp;
    public GameManager1 gameManager1;
    [Header("HP UI")]
    public Image hpGauge;
    public TextMeshProUGUI hpText;
    public GameObject damageTextPrefab;
    public GameObject TextSpawnTransform;
    private SpriteRenderer spriteRenderer;
    private Color originalColor;
    public PlayerMovement playerMovement;

    private Coroutine slowCoroutine;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        currentHp = maxHp;
        UpdateHpUI();
        originalColor = spriteRenderer.color;
    }

    public void TakeDamage(int amount, bool showText = true)
    {
        currentHp -= amount;
        currentHp = Mathf.Clamp(currentHp, 0, maxHp);
        UpdateHpUI();
        StartCoroutine(HitFlash());
        if (showText)
            ShowDamageText(amount);
        if (currentHp <= 0)
            Die();
    }

    private IEnumerator HitFlash()
    {
        spriteRenderer.color = new Color(1f, 0.3f, 0.3f, 0.7f);
        yield return new WaitForSeconds(0.2f);
        spriteRenderer.color = originalColor;
    }
    
    private void ShowDamageText(int damage)
    {
        ShowDamageText(damage, Color.white);
    }

    private void ShowDamageText(int damage, Color color)
    {
        if (damageTextPrefab == null) return;
        GameObject textObj = Instantiate(damageTextPrefab, TextSpawnTransform.transform.position, Quaternion.identity);
        DamageText text = textObj.GetComponent<DamageText>();
        if (text != null)
            text.SetText(damage.ToString(), color);
    }

    private void Update()
    {
        if (currentHp <= 0)
        {
            currentHp = 0;
        }
        UpdateHpUI();
        UpdateHpText();
        if (currentHp <= 0)
            Die();
    }

    public void Heal(int amount)
    {
        currentHp += amount;
        currentHp = Mathf.Clamp(currentHp, 0, maxHp);
        UpdateHpUI();
        UpdateHpText();
    }

    private void UpdateHpText()
    {
        hpText.text = $"{currentHp}/{maxHp}";
    }

    private void UpdateHpUI()
    {
        if (hpGauge != null)
            hpGauge.fillAmount = (float)currentHp / maxHp;
    }

    private void Die()
    {
        UpdateHpText();
        gameManager1.OnPlayerDeath();
        gameObject.SetActive(false);
    }

    // --- 도트딜(지속 피해) ---------------------------------------------------------------------------------------------------------------------------------------------
    public void ApplyDot(int damagePerTick, int tickCount, float tickInterval, Color textColor)
    {
        StartCoroutine(DotCoroutine(damagePerTick, tickCount, tickInterval, textColor));
    }

    private IEnumerator DotCoroutine(int damagePerTick, int tickCount, float tickInterval, Color textColor)
    {
        for (int i = 0; i < tickCount; i++)
        {
            ShowDamageText(damagePerTick, textColor);
            TakeDamage(damagePerTick, false);
            yield return new WaitForSeconds(tickInterval);
        }
    }

    public void ApplySlow(float slowMultiplier, float duration)
    {
        if (slowCoroutine != null)
        {
            StopCoroutine(slowCoroutine);
        }
        slowCoroutine = StartCoroutine(SlowCoroutine(slowMultiplier, duration));
    }

    private IEnumerator SlowCoroutine(float slowMultiplier, float duration)
    {
        if (playerMovement == null)
            yield break;

        float baseSpeed = playerMovement.baseSpeed;
        playerMovement.speed = baseSpeed * slowMultiplier;
        spriteRenderer.color = Color.cyan;
        yield return new WaitForSeconds(duration);
        spriteRenderer.color = originalColor;
        playerMovement.speed = baseSpeed;
        slowCoroutine = null;
    }
}
