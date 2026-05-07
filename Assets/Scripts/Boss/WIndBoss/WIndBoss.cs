using System.Collections;
using UnityEngine;

public class WIndBoss : MonoBehaviour
{
    [Header("플레이어 탐지 범위")]
    [SerializeField] private Vector2 detectBoxSize      = new Vector2(100f, 50f);
    [SerializeField] private Vector2 detectBoxOffset    = new Vector2(0f, 20f);

    [Header("공격 시도 범위")]
    [SerializeField] private Vector2 attackTryBoxSize   = new Vector2(18f, 8f);
    [SerializeField] private Vector2 attackTryBoxOffset = new Vector2(0f, 0f);

    [Header("공격 범위 1 (근접 공격 1)")]
    [SerializeField] private Vector2 attackBoxSize1     = new Vector2(10f, 5f);
    [SerializeField] private Vector2 attackBoxOffset1   = new Vector2(2f, 0f);

    [Header("공격 범위 2 (근접 공격 2)")]
    [SerializeField] private Vector2 attackBoxSize2     = new Vector2(8f,  4f);
    [SerializeField] private Vector2 attackBoxOffset2   = new Vector2(-2f, 0f);

    public LayerMask playerLayer;

    [Header("이동 / 공격")]
    public float moveSpeed = 3f;

    [Header("사운드 설정")]
    public AudioClip attack1Sound; // 공격1 효과음
    public AudioClip attack2Sound; // 공격2 효과음
    public AudioClip detectSound;  // 플레이어 감지 효과음
    private AudioSource audioSource;

    private Transform player;
    private bool playerDetected = false;
    private bool prevPlayerDetected = false; // 이전 프레임 감지 상태
    private bool canAttack      = true;
    private bool isAttacking    = false;
    private int  meleeAttackCount = 0;
    private bool isDead = false;

    private Animator       animator;
    private float cachedOffsetX1;
    private float cachedOffsetX2;
    private float cachedTryOffsetX;

    private WIndBossHealth bossHealth;
    private Coroutine attackCooldownCoroutine;

    private void Awake()
    {
        animator      = GetComponent<Animator>();
        bossHealth    = GetComponent<WIndBossHealth>();
        
        // AudioSource 초기화
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        if (bossHealth != null)
            bossHealth.OnBossDead += OnBossDead;
    }

    private void Update()
    {
        float dirSign = transform.localScale.x > 0 ? 1f : -1f;
        cachedOffsetX1      = dirSign * Mathf.Abs(attackBoxOffset1.x);
        cachedOffsetX2      = dirSign * Mathf.Abs(attackBoxOffset2.x);
        cachedTryOffsetX    = dirSign * Mathf.Abs(attackTryBoxOffset.x);

        if (!canAttack || isAttacking || isDead)
        {
            animator.SetBool("IsMoving", false);
            return;
        }

        // 1. 플레이어 탐지
        Collider2D detectHit = Physics2D.OverlapBox(
            (Vector2)transform.position + detectBoxOffset,
            detectBoxSize, 0, playerLayer
        );
        playerDetected = (detectHit != null);

        // ▶ 플레이어가 처음 범위에 들어왔을 때만 사운드 재생
        if (playerDetected && !prevPlayerDetected)
        {
            PlaySound(detectSound);
        }
        prevPlayerDetected = playerDetected;

        if (playerDetected)
        {
            player = detectHit.transform;
            transform.localScale = new Vector3(
                Mathf.Sign(player.position.x - transform.position.x),
                1, 1
            );

            // 2. 공격 시도 범위 체크
            bool inAttackTryRange = IsPlayerInAttackTryRange();
            animator.SetBool("IsMoving", !inAttackTryRange);

            if (!inAttackTryRange)
            {
                // 공격 시도 범위 밖이면 추적
                transform.position = Vector2.MoveTowards(
                    transform.position,
                    new Vector2(player.position.x, transform.position.y),
                    moveSpeed * Time.deltaTime
                );
            }

            // 3. 공격 시도 범위 안에 들어오면 공격 시도
            if (!isAttacking && canAttack && inAttackTryRange)
            {
                canAttack    = false;
                isAttacking  = true;

                // 원하는 공격 패턴: 공격1과 공격2를 번갈아 사용
                meleeAttackCount++;
                if (meleeAttackCount % 2 == 1)
                    animator.SetTrigger("Attack1");
                else
                    animator.SetTrigger("Attack2");
            }
        }
        else
        {
            animator.SetBool("IsMoving", false);
        }
    }

    private bool IsPlayerInAttackTryRange()
    {
        Vector2 center = (Vector2)transform.position + new Vector2(cachedTryOffsetX, attackTryBoxOffset.y);
        Collider2D hit = Physics2D.OverlapBox(center, attackTryBoxSize, 0, playerLayer);
        return hit != null;
    }

    private bool IsPlayerInRange(Vector2 size, Vector2 offset, float cachedOffsetX)
    {
        Vector2 center = (Vector2)transform.position + new Vector2(cachedOffsetX, offset.y);
        Collider2D hit = Physics2D.OverlapBox(center, size, 0, playerLayer);
        return hit != null;
    }

    // 공격1 애니메이션 이벤트에서 호출
    public void MeleeAttack1Event()
    {
        if (!playerDetected || isDead) return;

        // 공격1 효과음 재생
        PlaySound(attack1Sound);

        Vector2 center = (Vector2)transform.position + new Vector2(cachedOffsetX1, attackBoxOffset1.y);
        Collider2D hit = Physics2D.OverlapBox(center, attackBoxSize1, 0, playerLayer);
        if (hit != null)
        {
            PlayerHealth ph = hit.GetComponent<PlayerHealth>();
            if (ph != null)
            {
                ph.TakeDamage(20); 
            }
        }
    }

    // 공격2 애니메이션 이벤트에서 호출
    public void MeleeAttack2Event()
    {
        if (!playerDetected || isDead) return;

        // 공격2 효과음 재생
        PlaySound(attack2Sound);

        Vector2 center = (Vector2)transform.position + new Vector2(cachedOffsetX2, attackBoxOffset2.y);
        Collider2D hit = Physics2D.OverlapBox(center, attackBoxSize2, 0, playerLayer);
        if (hit != null)
        {
            PlayerHealth ph = hit.GetComponent<PlayerHealth>();
            if (ph != null)
            {
                ph.TakeDamage(30);
            }
        }
    }

    // 효과음 재생 메서드
    private void PlaySound(AudioClip clip)
    {
        if (clip != null && audioSource != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }

    public void StartAttackCooldown()
    {
        if (attackCooldownCoroutine != null)
            StopCoroutine(attackCooldownCoroutine);
        attackCooldownCoroutine = StartCoroutine(AttackCooldownCoroutine());
    }

    private IEnumerator AttackCooldownCoroutine()
    {
        animator.SetBool("IsMoving", false);
        yield return new WaitForSeconds(3f);
        canAttack = true;
    }

    public void AttackEndEvent()
    {
        isAttacking = false;
        if (!isDead)
            StartAttackCooldown();
    }

    public void TakeDamage(float damage)
    {
        if (bossHealth != null)
            bossHealth.TakeDamage(damage);
    }

    public void OnBossDead()
    {
        isDead = true;
        canAttack = false;
        isAttacking = false;
        animator.SetBool("IsMoving", false);

        if (attackCooldownCoroutine != null)
            StopCoroutine(attackCooldownCoroutine);
    }

    private void OnDrawGizmosSelected()
    {
        float gizmoX1 = Application.isPlaying 
            ? cachedOffsetX1 
            : (transform.localScale.x > 0 
                ? Mathf.Abs(attackBoxOffset1.x) 
                : -Mathf.Abs(attackBoxOffset1.x)
              );
        float gizmoX2 = Application.isPlaying 
            ? cachedOffsetX2 
            : (transform.localScale.x > 0 
                ? Mathf.Abs(attackBoxOffset2.x) 
                : -Mathf.Abs(attackBoxOffset2.x)
              );
        float gizmoTryX = Application.isPlaying 
            ? cachedTryOffsetX 
            : (transform.localScale.x > 0 
                ? Mathf.Abs(attackTryBoxOffset.x) 
                : -Mathf.Abs(attackTryBoxOffset.x)
              );

        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube((Vector2)transform.position + detectBoxOffset, detectBoxSize);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube((Vector2)transform.position + new Vector2(gizmoTryX, attackTryBoxOffset.y), attackTryBoxSize);

        Gizmos.color = Color.red;
        Gizmos.DrawWireCube((Vector2)transform.position + new Vector2(gizmoX1, attackBoxOffset1.y), attackBoxSize1);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube((Vector2)transform.position + new Vector2(gizmoX2, attackBoxOffset2.y), attackBoxSize2);
    }
}
