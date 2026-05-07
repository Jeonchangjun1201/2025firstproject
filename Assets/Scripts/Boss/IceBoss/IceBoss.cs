using UnityEngine;
using System.Collections;

public class IceBoss : MonoBehaviour
{
    [Header("추적 가능 범위")]
    [SerializeField] private Vector2 detectBoxSize      = new Vector2(100f, 50f);
    [SerializeField] private Vector2 detectBoxOffset    = new Vector2(0f, 20f);

    [Header("공격 시도 범위 (새로운 범위)")]
    [SerializeField] private Vector2 attackAttemptBoxSize = new Vector2(16f, 8f);
    [SerializeField] private Vector2 attackAttemptBoxOffset = new Vector2(0f, 0f);

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
    private bool canAttack      = true;   // 공격 가능 여부
    private bool isAttacking    = false;  // 공격 애니메이션 중 여부
    private int  meleeAttackCount = 0;    // 연속 공격 횟수(콤보)
    private bool isDead = false;

    private Animator       animator;
    private float cachedOffsetX1;
    private float cachedOffsetX2;

    private IceBossHealth bossHealth;
    private Coroutine attackCooldownCoroutine;

    private void Awake()
    {
        animator      = GetComponent<Animator>();
        bossHealth    = GetComponent<IceBossHealth>();
        audioSource   = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();
        if (bossHealth != null)
            bossHealth.OnBossDead += OnBossDead;
    }

    private void Update()
    {
        float dirSign = transform.localScale.x > 0 ? 1f : -1f;
        cachedOffsetX1 = dirSign * Mathf.Abs(attackBoxOffset1.x);
        cachedOffsetX2 = dirSign * Mathf.Abs(attackBoxOffset2.x);

        if (!canAttack || isAttacking || isDead)
        {
            animator.SetBool("IsMoving", false);
            return;
        }

        // 1. 플레이어 감지
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

            bool canTryAttack = IsPlayerInRange(attackAttemptBoxSize, attackAttemptBoxOffset, 0f);

            if (canTryAttack)
            {
                bool inRange = IsPlayerInAnyAttackRange();
                animator.SetBool("IsMoving", !inRange);

                if (!isAttacking && canAttack && inRange)
                {
                    canAttack    = false;
                    isAttacking  = true;

                    if (bossHealth != null && bossHealth.IsPhase2())
                    {
                        meleeAttackCount++;
                        if (meleeAttackCount < 3)
                        {
                            animator.SetTrigger("Attack1");
                        }
                        else
                        {
                            animator.SetTrigger("Attack2");
                            meleeAttackCount = 0;
                        }
                    }
                    else
                    {
                        animator.SetTrigger("Attack1");
                    }
                }
                else if (!inRange)
                {
                    transform.position = Vector2.MoveTowards(
                        transform.position,
                        new Vector2(player.position.x, transform.position.y),
                        moveSpeed * Time.deltaTime
                    );
                }
            }
            else
            {
                animator.SetBool("IsMoving", true);
                transform.position = Vector2.MoveTowards(
                    transform.position,
                    new Vector2(player.position.x, transform.position.y),
                    moveSpeed * Time.deltaTime
                );
            }
        }
        else
        {
            animator.SetBool("IsMoving", false);
        }
    }

    /// <summary>
    /// 모든 공격 범위(1, 2) 내 플레이어 존재 여부 체크
    /// </summary>
    private bool IsPlayerInAnyAttackRange()
    {
        return IsPlayerInRange(attackBoxSize1, attackBoxOffset1, cachedOffsetX1)
            || IsPlayerInRange(attackBoxSize2, attackBoxOffset2, cachedOffsetX2);
    }

    /// <summary>
    /// 단일 공격 범위 내 플레이어 존재 여부 체크
    /// </summary>
    private bool IsPlayerInRange(Vector2 size, Vector2 offset, float cachedOffsetX)
    {
        Vector2 center = (Vector2)transform.position + new Vector2(cachedOffsetX + offset.x, offset.y);
        Collider2D hit = Physics2D.OverlapBox(center, size, 0, playerLayer);
        return hit != null;
    }

    /// <summary>
    /// 애니메이션 이벤트: 근접 공격 1 데미지 적용 + 효과음
    /// </summary>
    public void MeleeAttack1Event()
    {
        if (!playerDetected || isDead) return;

        // 공격1 효과음 재생
        PlaySound(attack1Sound);

        Vector2 center = (Vector2)transform.position + new Vector2(cachedOffsetX1 + attackBoxOffset1.x, attackBoxOffset1.y);
        Collider2D hit = Physics2D.OverlapBox(center, attackBoxSize1, 0, playerLayer);
        if (hit != null)
        {
            PlayerHealth ph = hit.GetComponent<PlayerHealth>();
            if (ph != null)
            {
                ph.TakeDamage(20);
                ph.ApplyDot(3, 2, 0.7f, Color.cyan);
                ph.ApplySlow(0.5f, 2.5f);
            }
        }
    }

    /// <summary>
    /// 애니메이션 이벤트: 근접 공격 2 데미지 적용 + 효과음
    /// </summary>
    public void MeleeAttack2Event()
    {
        if (!playerDetected || isDead) return;

        // 공격2 효과음 재생
        PlaySound(attack2Sound);

        Vector2 center = (Vector2)transform.position + new Vector2(cachedOffsetX2 + attackBoxOffset2.x, attackBoxOffset2.y);
        Collider2D hit = Physics2D.OverlapBox(center, attackBoxSize2, 0, playerLayer);
        if (hit != null)
        {
            PlayerHealth ph = hit.GetComponent<PlayerHealth>();
            if (ph != null)
            {
                ph.TakeDamage(25);
                ph.ApplyDot(3, 2, 0.7f, Color.cyan);
                ph.ApplySlow(0.5f, 2.5f);
            }
        }
    }

    /// <summary>
    /// 효과음 재생 메서드
    /// </summary>
    private void PlaySound(AudioClip clip)
    {
        if (clip != null && audioSource != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }

    /// <summary>
    /// 애니메이션 이벤트: 공격 쿨타임 시작
    /// </summary>
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

    /// <summary>
    /// 애니메이션 이벤트: 공격 애니메이션 종료
    /// </summary>
    public void AttackEndEvent()
    {
        isAttacking = false;
        if (!isDead)
            StartAttackCooldown();
    }

    /// <summary>
    /// 외부에서 데미지를 입힐 때 IceBossHealth로 전달
    /// </summary>
    public void TakeDamage(float damage)
    {
        if (bossHealth != null)
            bossHealth.TakeDamage(damage);
    }

    /// <summary>
    /// 보스 사망시 호출: 모든 동작 즉시 중단
    /// </summary>
    public void OnBossDead()
    {
        isDead = true;
        canAttack = false;
        isAttacking = false;
        animator.SetBool("IsMoving", false);

        if (attackCooldownCoroutine != null)
            StopCoroutine(attackCooldownCoroutine);
    }

    /// <summary>
    /// 에디터용 기즈모 시각화
    /// </summary>
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

        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube((Vector2)transform.position + detectBoxOffset, detectBoxSize);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube((Vector2)transform.position + attackAttemptBoxOffset, attackAttemptBoxSize);

        Gizmos.color = Color.red;
        Gizmos.DrawWireCube((Vector2)transform.position + new Vector2(gizmoX1, attackBoxOffset1.y), attackBoxSize1);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube((Vector2)transform.position + new Vector2(gizmoX2, attackBoxOffset2.y), attackBoxSize2);
    }
}
