using System;
using System.Collections;
using NUnit.Framework.Constraints;
using UnityEngine;
using UnityEngine.UI;

public class EndingGem : MonoBehaviour
{
    public Sprite checkImage;
    public Image renderer;
    public Transform player;
    private float speed = 5.0f;
    private float followRange = 5.0f;
    private bool canGet = true;

    private void Start()//애매
    {
        player = GameObject.FindWithTag("Player").transform;
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

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Player") && canGet)
        {
            if (canGet)
            {
                renderer.sprite = checkImage;
                Destroy(gameObject);
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            if (canGet)
            {
                renderer.sprite = checkImage;
                GemManager.Instance.hasEndingGem = true;
                Destroy(gameObject);
            }
        }
    }
}