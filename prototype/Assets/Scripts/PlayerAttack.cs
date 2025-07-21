using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerAttack : MonoBehaviour
{
    [Header("Attack Settings")]
    public Transform attackPoint;
    public float attackRange = 0.5f;
    public LayerMask enemyLayers;
    public int baseDamage = 10;
    public float knockbackForce = 5f;
    public float comboResetTime = 1f;
    public Vector2 hitboxSize = new Vector2(1f, 1f); // For box-shaped hitbox
    
    [Header("Advanced")]
    public bool useBoxHitbox = false; // Toggle between circle and box hitbox
    public bool canChainCombo = true;
    public float hitPauseDuration = 0.1f; // Screen freeze on hit
    
    private Animator anim;
    private bool isAttacking;
    private int comboStep;
    private float lastAttackTime;
    private bool comboReady = true;
    
    void Awake()
    {
        anim = GetComponent<Animator>();
    }
    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.J) && CanAttack())
            StartAttack();

        CheckIfAttackFinished();
        CheckComboReset();
    }

    bool CanAttack()
    {
        return !isAttacking && comboReady;
    }

    void StartAttack()
    {
        isAttacking = true;
        lastAttackTime = Time.time;
        
        // Reset combo if too much time passed between attacks
        if (Time.time - lastAttackTime > comboResetTime)
            comboStep = 0;
            
        comboStep = (comboStep % 2) + 1;

        if (comboStep == 1) 
            anim.SetTrigger("attack1");
        else 
            anim.SetTrigger("attack2");
    }

    void CheckIfAttackFinished()
    {
        if (!isAttacking) return;

        AnimatorStateInfo st = anim.GetCurrentAnimatorStateInfo(0);
        
        if (st.IsTag("Attack") && st.normalizedTime >= 1f)
            isAttacking = false;
    }
    
    void CheckComboReset()
    {
        if (isAttacking) return;
        
        if (Time.time - lastAttackTime > comboResetTime)
        {
            comboStep = 0;
            comboReady = true;
        }
    }

    public void DealDamage()
    {
        Collider2D[] hits;
        
        if (useBoxHitbox)
        {
            hits = Physics2D.OverlapBoxAll(
                attackPoint.position, 
                hitboxSize, 
                0f, 
                enemyLayers);
        }
        else
        {
            hits = Physics2D.OverlapCircleAll(
                attackPoint.position, 
                attackRange, 
                enemyLayers);
        }

        bool hitConnected = false;
        
        foreach (var hit in hits)
        {
            // Calculate damage with potential combo multiplier
            int totalDamage = baseDamage;
            if (comboStep == 2) totalDamage = Mathf.RoundToInt(baseDamage * 1.3f);
            
            // Apply damage
            HealthSystem health = hit.GetComponent<HealthSystem>();
            if (health != null)
            {
                health.TakeDamage(totalDamage);
                hitConnected = true;
            }
            
            // Apply knockback
            KnockbackHandler knockback = hit.GetComponent<KnockbackHandler>();
            if (knockback != null)
            {
                Vector2 knockbackDirection = (hit.transform.position - transform.position).normalized;
                knockback.ReceiveHit(knockbackDirection, knockbackForce);
            }
        }
        
        // Hit pause effect
        if (hitConnected && hitPauseDuration > 0)
        {
            StartCoroutine(HitPauseEffect());
        }
        
        // Combo management
        if (!canChainCombo)
        {
            comboReady = false;
        }
    }
    
    System.Collections.IEnumerator HitPauseEffect()
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
        {
            Gizmos.DrawWireCube(attackPoint.position, hitboxSize);
        }
        else
        {
            Gizmos.DrawWireSphere(attackPoint.position, attackRange);
        }
    }
}
