using UnityEngine;
using System.Collections;
using TMPro;

public class FireBoss : MonoBehaviour
{
    [Header("추적 가능 범위")]
    [SerializeField] private Vector2 detectBoxSize = new Vector2(100f, 50f);
    [SerializeField] private Vector2 detectBoxOffset = new Vector2(0f, 20f);
    [Header("공격 가능 범위")]
    [SerializeField] private Vector2 attackableBoxSize = new Vector2(20f, 7f);
    [SerializeField] private Vector2 attackableBoxOffset = new Vector2(0f, 0f);
    [Header("공격 범위")]
    [SerializeField] private Vector2 attackBoxSize = new Vector2(10.4f, 4f);
    [SerializeField] private Vector2 attackBoxOffset = new Vector2(2f, -1.5f);

    public LayerMask playerLayer;

    [Header("이동/공격")]
    public float moveSpeed = 3f;
    public Transform firePos;
    public GameObject fireballPrefab;

    private Transform player;
    private bool playerDetected = false;
    private bool canAttack = true;
    private bool isAttacking = false;
    private int meleeAttackCount = 0;
    private Animator animator;
    private SpriteRenderer spriteRenderer;

    // 방향에 따라 미리 계산된 오프셋 값
    private float cachedAttackOffsetX;
    private float cachedAttackableOffsetX;

    public TextMeshProUGUI hpText;
    public GameObject hpGauge;
    public GameObject hpGaugeBackground;
    public GameObject hpBarPosition;

    private Coroutine attackCooldownCoroutine;

    [Header("사운드 설정")]
    public AudioClip[] meleeAttackSounds; // 근접 공격 효과음 배열
    public AudioClip playerDetectedSound; // 플레이어 감지 효과음
    private AudioSource audioSource;      // 오디오 소스

    private bool prevPlayerDetected = false; // 이전 프레임 감지 상태

    private void Awake()
    {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        // AudioSource 초기화
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    private void Update()
    {
        // 1. 방향에 따라 오프셋 미리 계산 (오른쪽: +, 왼쪽: -)
        cachedAttackOffsetX = transform.localScale.x > 0 ? Mathf.Abs(attackBoxOffset.x) : -Mathf.Abs(attackBoxOffset.x);
        cachedAttackableOffsetX = transform.localScale.x > 0 ? Mathf.Abs(attackableBoxOffset.x) : -Mathf.Abs(attackableBoxOffset.x);

        // 2. 쿨타임 또는 공격 중이면 아무것도 하지 않음
        if (!canAttack || isAttacking)
        { 
            animator.SetBool("isMoving", false);
            return;
        }

        // 3. 추적 가능 범위 체크
        Collider2D detectHit = Physics2D.OverlapBox((Vector2)transform.position + detectBoxOffset, detectBoxSize, 0, playerLayer);
        playerDetected = (detectHit != null);

        // ▶ 플레이어가 처음 범위에 들어왔을 때만 사운드 재생
        if (playerDetected && !prevPlayerDetected)
        {
            PlayPlayerDetectedSound();
        }
        prevPlayerDetected = playerDetected;

        if (playerDetected)
        {
            player = detectHit.transform;

            float dir = Mathf.Sign(player.position.x - transform.position.x);
            transform.localScale = new Vector3(dir, 1, 1);

            // 4. 이동 (공격 가능 범위 내에 없을 때만)
            bool shouldMove = !IsPlayerInAttackableRange();
            animator.SetBool("isMoving", shouldMove);

            if (shouldMove)
            {
                transform.position = Vector2.MoveTowards(transform.position, new Vector2(player.position.x, transform.position.y), moveSpeed * Time.deltaTime);
            }

            // 5. 공격 가능 범위 내에 있으면 공격 시도
            if (!isAttacking && canAttack && IsPlayerInAttackableRange())
            {
                canAttack = false;
                isAttacking = true;
                animator.SetTrigger("MeleeAttack");
                meleeAttackCount++;
                if (meleeAttackCount >= 3)
                {
                    animator.SetTrigger("RangedAttack");
                    meleeAttackCount = 0;
                }
            }
        }
        else
        {
            player = null;
            animator.SetBool("isMoving", false);
        }

        hpText.transform.position = hpBarPosition.transform.position;
        hpGauge.transform.position = hpBarPosition.transform.position;
        hpGaugeBackground.transform.position = hpBarPosition.transform.position;
    }

    /// <summary>
    /// 플레이어 감지 효과음 재생
    /// </summary>
    private void PlayPlayerDetectedSound()
    {
        if (playerDetectedSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(playerDetectedSound);
        }
    }

    /// <summary>
    /// 공격 가능 범위(OverlapBox) 체크
    /// </summary>
    private bool IsPlayerInAttackableRange()
    {
        Vector2 offset = new Vector2(cachedAttackableOffsetX, attackableBoxOffset.y);
        Collider2D hit = Physics2D.OverlapBox((Vector2)transform.position + offset, attackableBoxSize, 0, playerLayer);
        return hit != null;
    }

    /// <summary>
    /// 실제 공격 범위(OverlapBox) 체크 (애니메이션 이벤트에서 호출)
    /// </summary>
    private bool IsPlayerInAttackRange()
    {
        Vector2 offset = new Vector2(cachedAttackOffsetX, attackBoxOffset.y);
        Collider2D hit = Physics2D.OverlapBox((Vector2)transform.position + offset, attackBoxSize, 0, playerLayer);
        return hit != null;
    }

    /// <summary>
    /// 애니메이션 이벤트에서 호출: 근접 공격 데미지 적용 + 효과음 재생
    /// </summary>
    public void MeleeAttackEvent()
    {
        if (!playerDetected) return;
        Vector2 offset = new Vector2(cachedAttackOffsetX, attackBoxOffset.y);

        Collider2D hit = Physics2D.OverlapBox((Vector2)transform.position + offset, attackBoxSize, 0, playerLayer);
        if (hit != null)
        {
            PlayerHealth ph = hit.GetComponent<PlayerHealth>();
            if (ph != null)
            {
                ph.TakeDamage(15);
                ph.ApplyDot(2, 3, 0.5f, Color.red);
            }
        }

        // 근접 공격 효과음 재생 (랜덤)
        PlayRandomMeleeSound();
    }

    // 근접 공격 효과음 재생
    private void PlayRandomMeleeSound()
    {
        if (meleeAttackSounds == null || meleeAttackSounds.Length == 0 || audioSource == null) return;
        int index = Random.Range(0, meleeAttackSounds.Length);
        AudioClip clip = meleeAttackSounds[index];
        if (clip != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }

    /// <summary>
    /// 애니메이션 이벤트에서 호출: 원거리 공격(파이어볼 생성 및 방향 처리)
    /// </summary>
    public void FireballEvent()
    {
        if (!playerDetected || player == null) return;

        float dir = transform.localScale.x > 0 ? 1f : -1f;
        GameObject fb = Instantiate(fireballPrefab, firePos.position, Quaternion.identity);
        fb.GetComponent<FireBall>().SetTarget(player.position);
        var sr = fb.GetComponent<SpriteRenderer>();
        if (sr != null) sr.flipX = (dir > 0);
    }

    /// <summary>
    /// 애니메이션 이벤트에서 호출: 공격 쿨타임(3초) 시작(Idle 대기)
    /// </summary>
    public void StartAttackCooldown()
    {
        if (attackCooldownCoroutine != null)
        {
            StopCoroutine(attackCooldownCoroutine);
        }
        attackCooldownCoroutine = StartCoroutine(AttackCooldownCoroutine());
    }

    private IEnumerator AttackCooldownCoroutine()
    {
        yield return new WaitForSeconds(3f);
        canAttack = true;
    }

    /// <summary>
    /// 애니메이션 이벤트에서 호출: 공격 애니메이션 종료 시 호출 (마지막 프레임)
    /// </summary>
    public void AttackEndEvent()
    {
        isAttacking = false;
        StartAttackCooldown();
    }

    /// <summary>
    /// 감지 박스 시각화(에디터용)
    /// </summary>
    private void OnDrawGizmosSelected()
    {
        float gizmoAttackOffsetX = Application.isPlaying ? cachedAttackOffsetX : (transform.localScale.x > 0 ? Mathf.Abs(attackBoxOffset.x) : -Mathf.Abs(attackBoxOffset.x));
        float gizmoAttackableOffsetX = Application.isPlaying ? cachedAttackableOffsetX : (transform.localScale.x > 0 ? Mathf.Abs(attackableBoxOffset.x) : -Mathf.Abs(attackableBoxOffset.x));

        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube((Vector2)transform.position + detectBoxOffset, detectBoxSize);

        Vector2 atkAbleOffset = new Vector2(gizmoAttackableOffsetX, attackableBoxOffset.y);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube((Vector2)transform.position + atkAbleOffset, attackableBoxSize);

        Vector2 atkOffset = new Vector2(gizmoAttackOffsetX, attackBoxOffset.y);
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube((Vector2)transform.position + atkOffset, attackBoxSize);
    }
}
