using System.Collections;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    private float moveSpeed = 3.5f;
    [SerializeField] private bool moveRight = true;
    public Transform rightEdgeCheck;
    public Transform leftEdgeCheck;
    [SerializeField] float checkRadius = 0.2f;
    [SerializeField] private LayerMask groundLayer;
   
    private Enemy enemy;
    private Rigidbody2D rb;
    private SpriteRenderer sprite;

    void Start()
    {
        enemy = GetComponent<Enemy>();
        rb = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        if (enemy.isDead == false)
        {
            GameObject player = GameObject.FindWithTag("Player");
            if (player != null)
            {
                Vector2 playerPos = player.transform.position;
                Vector2 enemyPos = transform.position;

                bool withinYRange = playerPos.y >= enemyPos.y -3f && playerPos.y <= enemyPos.y + 3f;
                bool withinXRange = playerPos.x >= enemyPos.x -10f && playerPos.x <= enemyPos.x + 10f;

                if (withinYRange && withinXRange)
                {
                    Vector2 direction = (playerPos - enemyPos).normalized;
                    bool groundAhead = CheckGroundAhead(direction.x);
                    
                    if(groundAhead)
                    {
                        rb.linearVelocity = new Vector2(direction.x * moveSpeed, rb.linearVelocity.y);
                        sprite.flipX = (direction.x > 0);
                    }
                    else
                    {
                        rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
                    }
                }
                else
                {
                    Patrol();
                }
            }
        }
    }

    void Patrol()
    {
        bool atEdge = moveRight ? 
            Physics2D.OverlapCircle(rightEdgeCheck.position, checkRadius, groundLayer) :
            Physics2D.OverlapCircle(leftEdgeCheck.position, checkRadius, groundLayer);

        if (!atEdge) moveRight = !moveRight;
        
        rb.linearVelocity = new Vector2((moveRight ? 1 : -1) * moveSpeed, rb.linearVelocity.y);
        sprite.flipX = moveRight;
    }

    bool CheckGroundAhead(float directionX)
    {
        Vector2 checkPos = directionX > 0 ? 
            rightEdgeCheck.position : 
            leftEdgeCheck.position;
        return Physics2D.OverlapCircle(checkPos, checkRadius, groundLayer);
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            moveRight = !moveRight; // 방향 전환
            // 필요시 속도 즉시 변경
            rb.linearVelocity = new Vector2((moveRight ? 1 : -1) * moveSpeed, rb.linearVelocity.y);
        }
    }


    // OnCollisionEnter2D 및 기타 메서드는 동일
}
