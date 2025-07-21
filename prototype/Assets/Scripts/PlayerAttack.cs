using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [Header("Attack Settings")]
    public Transform attackPoint;
    public float attackRange = 0.5f;
    public LayerMask enemyLayers;
    public int damage = 10;
    public float knockbackForce = 5f;
    public float hitPauseDuration = 0.1f;
    public Vector2 boxSize = new Vector2(1f, 1f);
    public bool useBoxHitbox = false;

    private Animator anim;
    private bool isAttacking = false;

    void Awake()
    {
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.J) && !isAttacking)
        {
            Attack();
        }
    }

    void Attack()
    {
        isAttacking = true;
        anim.SetTrigger("attack");
    }

    // Call this via Animation Event at the hit frame
    public void DealDamage()
    {
        Collider2D[] hits;

        if (useBoxHitbox)
        {
            hits = Physics2D.OverlapBoxAll(attackPoint.position, boxSize, 0f, enemyLayers);
        }
        else
        {
            hits = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayers);
        }

        bool hitSomething = false;

        foreach (Collider2D hit in hits)
        {
            // Apply damage
            EnemyStats stats = hit.GetComponent<EnemyStats>();
            if (stats != null)
            {
                stats.TakeDamage(damage);
                hitSomething = true;
            }

            // Optional knockback
            KnockbackHandler knockback = hit.GetComponent<KnockbackHandler>();
            if (knockback != null)
            {
                Vector2 dir = (hit.transform.position - transform.position).normalized;
                knockback.ReceiveHit(dir, knockbackForce);
                hitSomething = true;
            }
        }

        if (hitSomething && hitPauseDuration > 0f)
        {
            StartCoroutine(HitPause());
        }
    }


    public void EndAttack() // Call this at end of attack animation
    {
        isAttacking = false;
    }

    IEnumerator HitPause()
    {
        Time.timeScale = 0.1f;
        yield return new WaitForSecondsRealtime(hitPauseDuration);
        Time.timeScale = 1f;
    }

    void OnDrawGizmosSelected()
    {
        if (!attackPoint) return;
        Gizmos.color = Color.red;

        if (useBoxHitbox)
            Gizmos.DrawWireCube(attackPoint.position, boxSize);
        else
            Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}
