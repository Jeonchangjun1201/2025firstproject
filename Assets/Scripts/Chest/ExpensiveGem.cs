using UnityEngine;
using UnityEngine.UI;

public enum GemType1 { Gem1, Gem2, Gem3 }

public class ExpensiveGem : MonoBehaviour
{
    public GemType1 gemType;
    public Sprite checkImage;
    public Image uiRenderer;
    
    private Transform player;
    private float speed = 5.0f;
    private bool canGet = true;

    private void OnEnable()
    {
        canGet = true;
    }

    private void Start()
    {
        player = GameObject.FindWithTag("Player").transform;
    }

    private void Update()
    {
        if (canGet && player != null)
        {
            Vector2 direction = (player.position - transform.position).normalized;
            transform.position = Vector2.MoveTowards(transform.position, player.position, speed * Time.deltaTime);
            
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && canGet)
        {
            CollectGem();
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Player") && canGet)
        {
            CollectGem();
        }
    }

    private void CollectGem()
    {
        canGet = false;
        uiRenderer.sprite = checkImage;

        // GemManager 업데이트
        switch (gemType)
        {
            case GemType1.Gem1:
                GemManager.Instance.hasGem1 = true;
                break;
            case GemType1.Gem2:
                GemManager.Instance.hasGem2 = true;
                break;
            case GemType1.Gem3:
                GemManager.Instance.hasGem3 = true;
                break;
        }

        Destroy(gameObject);
    }
}