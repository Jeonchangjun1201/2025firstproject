using UnityEngine;
using TMPro;

public class DamageText : MonoBehaviour
{
    public float moveSpeed = 2f;
    public float lifeTime = 1.5f;
    private float a;
    private Vector3 moveDir;
    private Vector3 direction;
    private TextMeshPro textMesh;

    void Awake()
    {
        textMesh = GetComponent<TextMeshPro>();
        a = Random.Range(-25f, 25f);
        Quaternion rotation = Quaternion.Euler(0, 0, a); //무작위 각도로 생성
        textMesh.rectTransform.rotation = rotation;
        direction = rotation * Vector3.up;
        Destroy(gameObject, lifeTime);
    }

    public void SetText(string damage)
    {
        if (textMesh != null)
            if (gameObject != null)
            {
                textMesh.text = damage;
            }
    }
    public void SetText(string damage, Color color)
    {
        if (textMesh != null && gameObject != null)
        {
            textMesh.text = damage;
            textMesh.color = color;
        }
    }

    void FixedUpdate()
    {
        if (gameObject != null)
        {
        transform.position += direction.normalized * moveSpeed * 0.01f;
        }
    }
}

