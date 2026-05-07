    using System;
    using System.Collections;
    using UnityEngine;
    using UnityEngine.SceneManagement;

    public class Shrine : MonoBehaviour
    {
        public Vector2 boxSize = new Vector2(2, 2); // 오버랩 박스 크기
        public LayerMask playerLayer;
        public KeyCode interactKey = KeyCode.E;
        SpriteRenderer spriteRenderer;
        public Sprite newSprite;

        private void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        void Update()
        {
            // 플레이어가 일정 거리 내에 있는지 체크
            Collider2D hit = Physics2D.OverlapBox(transform.position, boxSize, 0, playerLayer);
            if (hit != null && Input.GetKeyDown(interactKey))
            {
                // 엔딩 보석이 있는지 체크
                if (GemManager.Instance.hasEndingGem)
                {
                    if (GemManager.Instance.HasAllGems())
                    {
                        StartCoroutine(Next());
                    }
                    else
                    {
                        StartCoroutine(Nexti());
                    }
                }
                else
                {
                    // 엔딩 보석이 없을 때는 아무 일도 일어나지 않음
                    Debug.Log("엔딩 보석이 필요합니다.");
                }
            }
        }

        private IEnumerator Nexti()
        {
            transform.position = new Vector3(15.44f,0.58f,0.9267908f);
            spriteRenderer.sprite = newSprite;
            yield return new WaitForSeconds(0.5f);
            SceneManager.LoadScene("NormalEnding");
        }

        private IEnumerator Next()
        {
            transform.position = new Vector3(15.44f,0.58f,0.9267908f);
            spriteRenderer.sprite = newSprite;
            yield return new WaitForSeconds(0.5f);
            SceneManager.LoadScene("RichEnding");
        }

        // 오버랩 박스 시각화 (에디터에서만)
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireCube(transform.position, boxSize);
        }
    }