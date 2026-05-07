using System.Collections;
using UnityEngine;

public enum ElementType
{
    Default,    // 기본
    Blue,      // 갈색(슬로우)
    Red,        // 빨간색(불 도트딜)
    Yellow,     // 노란색(전기 도트딜)
    Purple      // 보라색(독 도트딜)
}

public class EnemyAttack : MonoBehaviour
{
    [Header("PlayerCheck")]
    public Vector2 detectBoxSize = new Vector2(1.5f, 1f);
    public Vector2 detectBoxOffset = new Vector2(1f, 0f);

    [Header("AttackCheck")]
    public Vector2 attackBoxSize = new Vector2(2f, 1.2f);
    public Vector2 attackBoxOffset = new Vector2(1.2f, 0f);

    public LayerMask playerLayer;
    public ElementType elementType = ElementType.Default; // Inspector에서 지정

    [Header("사운드 설정")]
    public AudioClip attackSound;      // 공격 효과음
    private AudioSource audioSource;   // 오디오 소스

    private SpriteRenderer spriteRenderer;
    private Animator animator;
    private bool canAttack = true;
    public float attackCooldown = 1.5f;
    private Enemy enemy;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        enemy = GetComponent<Enemy>();

        // AudioSource 초기화
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();
    }

    void Update()
    {
        if (!enemy.isDead)
        {
            if (canAttack && IsPlayerInDetectRange())
            {
                canAttack = false;
                animator.SetTrigger("Attack");
            }
        }
    }

    // 공격 감지 범위
    bool IsPlayerInDetectRange()
    {
        float direction = spriteRenderer.flipX ? 1f : -1f;
        Vector2 boxCenter = (Vector2)transform.position + new Vector2(detectBoxOffset.x * direction, detectBoxOffset.y);
        Collider2D hit = Physics2D.OverlapBox(boxCenter, detectBoxSize, 0, playerLayer);
        return hit != null;
    }

    // 공격 판정 범위(실제 데미지, 애니메이션 이벤트에서 호출)
    public void AttackDamageEvent()
    {
        // 공격 효과음 재생
        PlayAttackSound();

        float direction = spriteRenderer.flipX ? 1f : -1f;
        Vector2 boxCenter = (Vector2)transform.position + new Vector2(attackBoxOffset.x * direction, attackBoxOffset.y);
        Collider2D hit = Physics2D.OverlapBox(boxCenter, attackBoxSize, 0, playerLayer);
        if (hit != null)
        {
            PlayerHealth hp = hit.GetComponent<PlayerHealth>();
            if (hp != null)
            {
                hp.TakeDamage(5);

                switch (elementType)
                {
                    case ElementType.Blue:
                        hp.ApplyDot(3, 2, 0.7f, Color.cyan); // 도트딜
                        hp.ApplySlow(0.5f, 2.5f);              // 슬로우
                        break;
                    case ElementType.Red:
                        hp.ApplyDot(2, 3, 0.5f, Color.red);
                        break;
                    case ElementType.Yellow:
                        hp.ApplyDot(1, 3, 0.3f, Color.yellow);
                        break;
                    case ElementType.Purple:
                        hp.ApplyDot(1, 5, 0.7f, new Color(0.6f, 0.2f, 0.8f));
                        break;
                    case ElementType.Default:
                        // 아무 효과 없음
                        break;
                }
            }
        }
    }

    // 공격 효과음 재생 함수
    private void PlayAttackSound()
    {
        if (attackSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(attackSound);
        }
    }

    // 공격 쿨타임 (애니메이션 이벤트에서 호출)
    public void OnAttackEnd()
    {
        StartCoroutine(AttackCooldown());
    }

    private IEnumerator AttackCooldown()
    {
        yield return new WaitForSeconds(attackCooldown);
        canAttack = true;
    }

    // 기즈모로 두 범위 모두 시각화
    void OnDrawGizmosSelected()
    {
        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();

        float direction = spriteRenderer != null && spriteRenderer.flipX ? 1f : -1f;

        // 감지 범위
        Vector2 detectCenter = (Vector2)transform.position + new Vector2(detectBoxOffset.x * direction, detectBoxOffset.y);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(detectCenter, detectBoxSize);

        // 공격 판정 범위
        Vector2 attackCenter = (Vector2)transform.position + new Vector2(attackBoxOffset.x * direction, attackBoxOffset.y);
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(attackCenter, attackBoxSize);
    }
}
