using System;
using Unity.VisualScripting;
using UnityEngine;

using UnityEngine;
using UnityEngine.Events;

public class DoorChecker : MonoBehaviour
{
    [SerializeField] private GameObject gaugeGimic; // LockGamePanel 등 UI 오브젝트
    private bool inDoor = false;
    private MovingBarGauge gauge;
    public UnityEvent tryOpen;
    private Chest chest;
    private bool canOpen = true;
    private void Start()
    {
        chest = GetComponent<Chest>();
        if (gaugeGimic != null)
        {
            gauge = gaugeGimic.GetComponent<MovingBarGauge>();
            if (gauge == null)
                Debug.LogError($"{gaugeGimic.name}에 MovingBarGauge 컴포넌트가 없습니다!");
        }
        else
        {
            Debug.LogError("gaugeGimic이 할당되지 않았습니다!");
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            inDoor = true;
            if (gauge != null)
                gauge.SetChest(this.gameObject); // Metal Chest를 UI에 전달
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
            inDoor = false;
    }   

    private void Update()
    {
        canOpen = chest.canOpen;
        if (inDoor && Input.GetKeyDown(KeyCode.E)&& canOpen)
        {
            if (gaugeGimic != null)
                chest.TryOpenChest();
            gaugeGimic.SetActive(true);
        }
    }
}

