using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class AttackAnimation : MonoBehaviour
{
    private Animator animator;
    private bool attacking = false;
    private int attackCount = 0;
    private bool comboQueued = false;
    private bool canCombo = false;

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            if (!attacking)
            {
                Attacked();
            }
            else if (canCombo) // 콤보 윈도우에서만 입력 받기
            {
                comboQueued = true;
            }
        }
    }
    private void Attacked()
    {
        attacking = true;
        StartCoroutine(Attacki());
    }

    private IEnumerator Attacki()
    {
        attackCount++;
        if (attackCount > 3) attackCount = 1;
        animator.SetInteger("AttackCount", attackCount);

        float waitTime = (attackCount == 1) ? 0.4f : 0.5f;
        canCombo = true;
        yield return new WaitForSeconds(waitTime); // 콤보 입력 가능한 구간
        canCombo = false;
        if (comboQueued)
        {
            comboQueued = false;
            StartCoroutine(Attacki());
        }
        else
        {
            attacking = false;
            attackCount = 0;
            animator.SetInteger("AttackCount", 0);
        }
    }
}
