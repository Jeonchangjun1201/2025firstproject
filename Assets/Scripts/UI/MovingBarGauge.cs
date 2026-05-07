using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class MovingBarGauge : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private Image gauge;
    [SerializeField] private RectTransform bar;
    [SerializeField] private RectTransform line;
    [SerializeField] private RectTransform gaugeBar;
    [SerializeField] private GameObject key;
    [SerializeField] private Sprite emptyKey;
    [SerializeField] private Sprite fullKey;

    [Header("Speed Setting")]
    [SerializeField] private float barSpeed = 300f;
    [SerializeField] private float lineMinSpeed = 200f;
    [SerializeField] private float lineMaxSpeed = 210f;

    [Header("Gauge")]
    private float gaugeFillSpeed = 0.4f;
    [SerializeField] private float gaugeDecreaseSpeed = 0.3f;
    [SerializeField] private float gaugeLerpSpeed = 5f;

    [Header("Item Spawning")]
    [SerializeField] private GameObject[] itemPrefabs;
    [SerializeField] private int minItems = 3;
    [SerializeField] private int maxItems = 7;
    [SerializeField] private float spawnRadius = 2f;
    [SerializeField] private float spawnForce = 5f;

    private float gaugeValue = 0f;
    private float gaugeVisual = 0f;
    private float gaugeBarWidth;
    private float barWidth;
    private float lineWidth;

    private float lineTargetX;
    private float lineMoveSpeed;
    private float lineChangeTimer = 0f;
    private float lineChangeInterval = 0.3f;

    private Image image;
    private bool canMove = true;
    private bool isCompleted = false;

    public UnityEvent Open;

    // 상자 참조
    private GameObject targetChest;
    private Animator chestAnimator;
    private SpriteRenderer chestSpriteRenderer;

    private void Awake()
    {
        if (key != null)
            image = key.GetComponent<Image>();
    }

    private void OnEnable()
    {
        MinigameState.IsMinigameActive = true; // 미니게임 시작
        Time.timeScale = 0f; // 미니게임 시작 시 전체 일시정지
        InitializeUI();
        ResetGauge();
    }

    private void OnDisable()
    {
        MinigameState.IsMinigameActive = false; // 미니게임 종료
        Time.timeScale = 1f; // 미니게임 종료 시 전체 재개
    }

    private void Start()
    {
        InitializeUI();
    }

    private void InitializeUI()
    {
        if (gaugeBar != null)
            gaugeBarWidth = gaugeBar.rect.width;
        if (bar != null)
            barWidth = bar.rect.width;
        if (line != null)
            lineWidth = line.rect.width;
        SetRandomLineTarget();
        if (image != null)
            image.sprite = emptyKey;
        if (gauge != null)
            gauge.fillAmount = 0f;
    }

    private void ResetGauge()
    {
        gaugeValue = 0f;
        gaugeVisual = 0f;
        canMove = true;
        isCompleted = false;
        SetRandomLineTarget();
        if (image != null)
            image.sprite = emptyKey;
        if (bar != null)
            bar.anchoredPosition = new Vector2(0, bar.anchoredPosition.y);
        if (line != null)
            line.anchoredPosition = new Vector2(0, line.anchoredPosition.y);
        if (gauge != null)
            gauge.fillAmount = 0f;
    }

    public void SetChest(GameObject chest)
    {
        targetChest = chest;
        if (targetChest != null)
        {
            chestAnimator = targetChest.GetComponent<Animator>();
            chestSpriteRenderer = targetChest.GetComponent<SpriteRenderer>();
            if (chestAnimator == null)
                Debug.LogError($"{targetChest.name}에 Animator가 없습니다!");
            if (chestSpriteRenderer == null)
                Debug.LogError($"{targetChest.name}에 SpriteRenderer가 없습니다!");
        }
    }

    private void Update()
    {
        if (canMove && !isCompleted)
        {
            HandleBarMovement();
            HandleLineMovement();
            CheckGauge();
            UpdateGaugeVisual();
            if (gaugeVisual >= 0.99f && !isCompleted)
            {
                isCompleted = true;
                StartCoroutine(OpenChestSequence());
            }
        }
    }

    private void HandleBarMovement()
    {
        float barX = bar.anchoredPosition.x;
        if (Input.GetMouseButton(0))
            barX += barSpeed * Time.unscaledDeltaTime;
        else
            barX -= barSpeed * Time.unscaledDeltaTime;
        float minBarX = -gaugeBarWidth / 2 + barWidth / 2;
        float maxBarX = gaugeBarWidth / 2 - barWidth / 2;
        barX = Mathf.Clamp(barX, minBarX, maxBarX);
        bar.anchoredPosition = new Vector2(barX, bar.anchoredPosition.y);
    }

    private void HandleLineMovement()
    {
        lineChangeTimer -= Time.unscaledDeltaTime;
        if (lineChangeTimer <= 0f)
        {
            SetRandomLineTarget();
            lineChangeTimer = lineChangeInterval;
        }

        float lineX = Mathf.MoveTowards(line.anchoredPosition.x, lineTargetX, lineMoveSpeed * Time.unscaledDeltaTime);
        float minLineX = -gaugeBarWidth / 2 + lineWidth / 2;
        float maxLineX = gaugeBarWidth / 2 - lineWidth / 2;
        lineX = Mathf.Clamp(lineX, minLineX, maxLineX);
        line.anchoredPosition = new Vector2(lineX, line.anchoredPosition.y);
    }

    private void CheckGauge()
    {
        float barLeft = bar.anchoredPosition.x - barWidth / 2;
        float barRight = bar.anchoredPosition.x + barWidth / 2;
        float lineLeft = line.anchoredPosition.x - lineWidth / 2;
        float lineRight = line.anchoredPosition.x + lineWidth / 2;
        bool isLineInBar = lineRight > barLeft && lineLeft < barRight;
        if (isLineInBar)
            gaugeValue += Time.unscaledDeltaTime * gaugeFillSpeed;
        else
            gaugeValue -= Time.unscaledDeltaTime * gaugeDecreaseSpeed;
        gaugeValue = Mathf.Clamp01(gaugeValue);
    }

    private void UpdateGaugeVisual()
    {
        gaugeVisual = Mathf.Lerp(gaugeVisual, gaugeValue, Time.unscaledDeltaTime * gaugeLerpSpeed);
        if (gauge != null)
            gauge.fillAmount = gaugeVisual;
    }

    private void SetRandomLineTarget()
    {
        float minLineX = -gaugeBarWidth / 2 + lineWidth / 2;
        float maxLineX = gaugeBarWidth / 2 - lineWidth / 2;
        lineTargetX = Random.Range(minLineX, maxLineX);
        lineMoveSpeed = Random.Range(lineMinSpeed, lineMaxSpeed);
    }

    private IEnumerator OpenChestSequence()
    {
        canMove = false;
        if (image != null)
            image.sprite = fullKey;
        yield return new WaitForSecondsRealtime(0.2f);

        if (chestAnimator != null)
            chestAnimator.SetBool("Open", true);
        OnDisable();

        Open?.Invoke();
        gameObject.SetActive(false);
    }
}
