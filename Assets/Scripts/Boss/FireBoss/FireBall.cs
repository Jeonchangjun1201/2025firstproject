using UnityEngine;

/// <summary>
/// FireBall 투사체 스크립트
/// - 인스턴스화 시 플레이어 위치를 받아 방향과 각도를 한 번만 설정
/// - 바닥(오른쪽)과 플레이어 위치 사이의 각도에 맞춰 회전
/// - 이후 방향은 변하지 않고 쭉 날아감
/// - flipX는 사용하지 않음(회전만 적용)
/// </summary>
public class FireBall : MonoBehaviour
{
    public float speed = 8f;
    public int damage = 15;
    public float lifeTime = 3f;

    private Vector2 direction;
    private Animator animator;
    private bool isMove = true;
    private SpriteRenderer spriteRenderer;

    void Awake()
    {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    /// <summary>
    /// 플레이어 위치를 받아 방향과 각도를 한 번만 설정
    /// </summary>
    public void SetTarget(Vector2 playerPosition)
    {
        Vector2 direction = (playerPosition - (Vector2)transform.position).normalized;
        float angle;

        if (playerPosition.x < transform.position.x) // 플레이어가 왼쪽
        {
            angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        }
        else // 플레이어가 오른쪽
        {
            angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg + 180f;
        }
        transform.rotation = Quaternion.Euler(0, 0, angle);

        if (spriteRenderer != null)
            spriteRenderer.flipX = direction.x < 0;

        this.direction = direction;
    }

    void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    void Update()
    {
        if (isMove)
        {
            // 월드 좌표 기준으로 방향 이동
            transform.Translate(direction * speed * Time.deltaTime, Space.World);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        isMove = false;
        if (collision.CompareTag("Player"))
        {
            PlayerHealth hp = collision.GetComponent<PlayerHealth>();
            if (hp != null)
            {
                hp.TakeDamage(damage);
                hp.ApplyDot(2, 3, 0.5f, Color.red); // 불 도트딜 효과
            }
        }
        animator.SetBool("Explosion", true);
    }

    /// <summary>
    /// 애니메이션 이벤트에서 호출: 파이어볼 오브젝트 제거
    /// </summary>
    public void GameObjectDestroy()
    {
        Destroy(gameObject);
    }
}