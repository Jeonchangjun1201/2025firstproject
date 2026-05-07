using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ShrineTutorial : MonoBehaviour
{
    public Vector2 boxSize = new Vector2(2, 2); // 오버랩 박스 크기
    public LayerMask playerLayer;
    public KeyCode interactKey = KeyCode.E;
    public Sprite newSprite;
    private SpriteRenderer spriteRenderer;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        // 플레이어가 일정 거리 내에 있는지 체크
        Collider2D hit = Physics2D.OverlapBox(transform.position, boxSize, 0, playerLayer);
        if (hit != null && Input.GetKeyDown(interactKey))
        {
            StartCoroutine(NextScene());
                    
        }
    }

    private IEnumerator NextScene()
    {
        transform.position = new Vector3(243.8f, 0.66f, 0.9267908f);
        spriteRenderer.sprite = newSprite;
        yield return null; // 한 프레임 대기
        yield return new WaitForSeconds(0.5f);
        SceneManager.LoadScene("TitleScene");
    }

    // 오버랩 박스 시각화 (에디터에서만)
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(transform.position, boxSize);
    }
}