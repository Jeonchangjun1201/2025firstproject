using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public enum ChestType
{
    Normal, // 일반 상자(코인/포션/잭팟)
    Gem     // 보석 상자(보석만)
}

public enum GemType
{
    None,
    EndingGem,
    ExpensiveGem1,
    ExpensiveGem2,
    ExpensiveGem3
}

public class Chest : MonoBehaviour
{
    [Header("상자 타입 설정")]
    public ChestType chestType = ChestType.Normal;

    [Header("일반 상자 설정")]
    public bool isJackpot = false;
    public GameObject coinPrefab;
    public GameObject potionPrefab;
    public int minCoins = 2;
    public int maxCoins = 5;
    public float spawnRadius = 2f;
    public float spawnForce = 5f;

    [Header("보석 상자 설정 (외부에서 할당)")]
    [HideInInspector] public GameObject assignedGemPrefab; // 매니저에서 할당

    [Header("젬상자에서 열릴 포탈")]
    [SerializeField] private GameObject portalObject; // Inspector에서 할당

    [Header("기존 보석 세팅 (호환성용)")]
    public GemType gemType = GemType.None;
    public GameObject endingGemObject;
    public GameObject expensiveGem1Object;
    public GameObject expensiveGem2Object;
    public GameObject expensiveGem3Object;

    [Header("게이지 (필요시)")]
    public MovingBarGauge barGauge;

    [Header("사운드 설정")]
    public AudioClip chestOpenSound;     // 상자 열리는 소리
    private AudioSource audioSource;      // 오디오 소스

    public bool canOpen = true;
    private bool opened = false;

    private Animator animator;

    void Awake()
    {
        // AudioSource 초기화
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        animator = GetComponent<Animator>();
    }

    public void TryOpenChest()
    {
        if (barGauge != null)
        {
            barGauge.Open.RemoveAllListeners();
            barGauge.Open.AddListener(Open);
        }
        else
        {
            Open();
        }
    }

    public void Open()
    {
        if (!canOpen || opened) return;
        opened = true;

        // 상자 열리는 소리만 재생
        PlaySound(chestOpenSound);

        // 상자 열기 애니메이션 재생
        if (animator != null)
            animator.SetTrigger("Open"); // "Open" 트리거를 애니메이션에 맞게 설정

        // 아이템 생성은 애니메이션 이벤트(SpawnItems)에서 처리!
        StartCoroutine(Destroye());
    }

    // 애니메이션 이벤트에서 호출
    public void SpawnItems()
    {
        // 보석 상자
        if (chestType == ChestType.Gem)
        {
            if (assignedGemPrefab != null)
            {
                assignedGemPrefab.transform.position = transform.position;
                assignedGemPrefab.SetActive(true);
            }
            if (portalObject != null)
            {
                StartCoroutine(ActivatePortalAfterDelay(0.5f));
            }
        }
        // 일반 상자
        else if (chestType == ChestType.Normal)
        {
            if (isJackpot)
            {
                for (int i = 0; i < 10; i++)
                    SpawnScatteredItem(coinPrefab);
                return;
            }

            if (Random.value < 0.15f)
                SpawnScatteredItem(potionPrefab);

            int coinCount = Random.Range(minCoins, maxCoins + 1);
            for (int i = 0; i < coinCount; i++)
                SpawnScatteredItem(coinPrefab);
        }
    }

    // 소리 재생 메서드
    private void PlaySound(AudioClip clip)
    {
        if (clip != null && audioSource != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }

    void SpawnScatteredItem(GameObject prefab)
    {
        if (prefab == null) return;
        Vector2 offset = Random.insideUnitCircle * spawnRadius;
        Vector3 spawnPos = transform.position + new Vector3(offset.x, offset.y, 0);
        GameObject item = Instantiate(prefab, spawnPos, Quaternion.identity);
        Rigidbody2D rb = item.GetComponent<Rigidbody2D>();
        if (rb != null)
            rb.AddForce(offset.normalized * spawnForce, ForceMode2D.Impulse);
    }

    private IEnumerator ActivatePortalAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        portalObject.SetActive(true);
    }

    private IEnumerator Destroye()
    {
        canOpen = false;
        yield return new WaitForSeconds(1.375f);
        Destroy(gameObject);
    }
}
