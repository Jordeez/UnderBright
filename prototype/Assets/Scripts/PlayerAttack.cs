using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerAttack : MonoBehaviour
{
    [Header("Hitbox")]
    public Transform attackPoint;
    public float attackRange = 0.5f;
    public LayerMask enemyLayers;

    
    Animator anim;
    bool isAttacking;
    int comboStep;           

    void Awake() => anim = GetComponent<Animator>();

    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.J) && !isAttacking)
            StartAttack();

        CheckIfAttackFinished();
    }

    void StartAttack()
    {
        isAttacking = false;
        comboStep = (comboStep % 2) + 1;   

        if (comboStep == 1) anim.SetTrigger("attack1");
        else anim.SetTrigger("attack2");
    }

    
    void CheckIfAttackFinished()
    {
        if (!isAttacking) return;

        AnimatorStateInfo st = anim.GetCurrentAnimatorStateInfo(0);

        
        if (st.IsTag("Attack") && st.normalizedTime >= 1f)
            isAttacking = false;
    }

   
    public void DealDamage()
    {
        foreach (var hit in Physics2D.OverlapCircleAll(
                     attackPoint.position, attackRange, enemyLayers))
            hit.GetComponent<KnockbackHandler>()?.ReceiveHit(transform.position);
    }


    void OnDrawGizmosSelected()
    {
        if (!attackPoint) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}
