using System;
using UnityEngine;

public class Portal : MonoBehaviour
{
    public Transform startPoint; // 시작지점(이동할 위치) - Inspector에서 할당
    private bool playerInTrigger = false;
    private GameObject playerObj;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInTrigger = true;
            playerObj = other.gameObject;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInTrigger = false;
            playerObj = null;
        }
    }

    private void Update()
    {
        if (playerInTrigger && playerObj != null)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                // 플레이어 위치를 시작지점으로 이동
                playerObj.transform.position = startPoint.position;
            }
        }
    }
}
