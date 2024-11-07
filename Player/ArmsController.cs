using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ArmsController : MonoBehaviour
{
    [SerializeField] 
    private Animator ArmsAnimator;

    [SerializeField]
    UnityEvent OnAttack = new UnityEvent();

    public void OnChangedRunning(bool IsRunning)
    {
        ArmsAnimator.SetBool("isrunning", IsRunning);
    }

    public void OnChangedMoving(bool IsMoving)
    {
        ArmsAnimator.SetBool("ismoving", IsMoving);
    }

    public void OnChangedAttack(bool IsAttacking)
    {
        ArmsAnimator.SetBool("isattacking", IsAttacking);

        if (IsAttacking)
        {
            int randomAttackAnimation = Random.Range(0, 3);

            ArmsAnimator.SetInteger("whichattack", randomAttackAnimation);
        }
    }

    public void Attack()
    {
        OnAttack.Invoke();
    }

    public void EndAttackAnimation()
    {
        int randomAttackAnimation = Random.Range(0, 3);

        ArmsAnimator.SetInteger("whichattack", randomAttackAnimation);
    }
}
