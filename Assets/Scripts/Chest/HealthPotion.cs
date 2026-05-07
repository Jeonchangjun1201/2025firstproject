using System.Collections;
using UnityEngine;

public class HealthPotion : MonoBehaviour
{
    public int healAmount = 30;
    private bool canFollow = false;
    private Transform player;
    private float speed = 5.0f;
    private float followRange = 5.0f;
    private bool canGet = false;
    private void Start()
    {
        player = GameObject.FindWithTag("Player").transform;
        StartCoroutine(Get());
    }

    private IEnumerator Get()
    {
        yield return new WaitForSeconds(0.3f);
        canGet = true;
    }

    private void Update()
    {
        player = GameObject.FindWithTag("Player").transform;
        if (canGet)
        {
            
            Vector2 direction = (player.position - transform.position).normalized;
            transform.position = Vector2.MoveTowards(transform.position, player.position, speed * Time.deltaTime);
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg; 
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
            
        }
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && canGet)
        {
            PotionInventory.Instance.AddPotion(1);
            // TODO: 이펙트/사운드 추가
            Destroy(gameObject);
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (canGet)
            {
                PotionInventory.Instance.AddPotion(1);
                // TODO: 이펙트/사운드 추가
                Destroy(gameObject);
            }
        }
    }
    
}
