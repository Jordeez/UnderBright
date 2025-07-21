using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Animator), typeof(EnemyAI))]
public class EnemyChase_Attack : MonoBehaviour
{
    [Header("Detection Settings")]
    [SerializeField] private float detectionRange = 5f;
    [SerializeField] private float attackRange = 1f;
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] [Range(0,1)] private float rearDetectionReduction = 0.5f;

    [Header("Chase Settings")]
    [SerializeField] private float chaseSpeed = 3f;
    [SerializeField] private float stoppingDistance = 0.5f;
    
    [Header("Attack Settings")]
    [SerializeField] private float attackCooldown = 2f;
    [SerializeField] private float chargeTime = 0.5f;
    [SerializeField] private int attackDamage = 1;
    [SerializeField] private Vector2 attackSize = new Vector2(1.5f, 1.5f);
    [SerializeField] private float attackKnockback = 5f;
    [SerializeField] private float hitFreezeDuration = 0.1f;
    [SerializeField] private GameObject hitEffectPrefab;

    // Components
    private Rigidbody2D rb;
    private Animator anim;
    private EnemyAI enemyAI;
    private Transform player;

    // State variables
    private float lastAttackTime;
    private bool isChargingAttack;
    private float chargeStartTime;
    private bool wasPatrolling;
    public Vector2 chargeDirection;

    public void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        enemyAI = GetComponent<EnemyAI>();
        player = GameObject.FindWithTag("Player")?.transform;
    }

    public void Update()
    {
        if (player == null) return;

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);
        bool playerInFront = IsPlayerInFront();

        float effectiveDetectionRange = playerInFront ? detectionRange : detectionRange * rearDetectionReduction;

        if (isChargingAttack)
        {
            // Raycast forward to detect walls
            RaycastHit2D hit = Physics2D.Raycast(
                transform.position, 
                chargeDirection, 
                0.5f, 
                LayerMask.GetMask("Ground"));

            if (hit.collider != null)
            {
                isChargingAttack = false;
                rb.linearVelocity = Vector2.zero;
            }
        }
        
        if (distanceToPlayer <= effectiveDetectionRange)
        {
            // Disable patrolling when player is detected
            if (enemyAI.enabled)
            {
                wasPatrolling = true;
                enemyAI.enabled = false;
            }

            if (ShouldAttack(distanceToPlayer))
            {
                HandleAttack();
            }
            else if (!isChargingAttack)
            {
                ChasePlayer(distanceToPlayer);
            }
        }
        else
        {
            // Re-enable patrolling when player is out of range
            if (wasPatrolling)
            {
                enemyAI.enabled = true;
                wasPatrolling = false;
            }
            Idle();
        }

        UpdateAnimations();
    }

    public bool IsPlayerInFront()
    {
        Vector2 toPlayer = (player.position - transform.position).normalized;
        return Vector2.Dot(toPlayer, transform.right) > 0;
    }

    public bool ShouldAttack(float distanceToPlayer)
    {
        return distanceToPlayer <= attackRange && 
               Time.time > lastAttackTime + attackCooldown;
    }

    public void HandleAttack()
    {
        if (!isChargingAttack)
        {
            StartCharge();
        }
        else if (Time.time >= chargeStartTime + chargeTime)
        {
            ExecuteAttack();
        }
        else
        {
            // Keep moving during charge
            rb.linearVelocity = chargeDirection * chaseSpeed * 2f;
        }
    }

    public void StartCharge()
    {
        isChargingAttack = true;
        chargeStartTime = Time.time;
        chargeDirection = (player.position - transform.position).normalized;
        rb.linearVelocity = chargeDirection * chaseSpeed * 2f;
        anim.SetTrigger("StartCharge");
    }

    public void ExecuteAttack()
    {
        lastAttackTime = Time.time;
        isChargingAttack = false;
        anim.SetTrigger("ExecuteAttack");

        Vector2 attackPos = (Vector2)transform.position + chargeDirection * (attackSize.x * 0.6f);
        
        Collider2D[] hitPlayers = Physics2D.OverlapBoxAll(
            attackPos, 
            attackSize, 
            Vector2.Angle(Vector2.right, chargeDirection), 
            playerLayer);

        bool hitConfirmed = false;
        
        foreach (Collider2D playerCollider in hitPlayers)
        {
            if (playerCollider.CompareTag("Player"))
            {
                // Apply damage
                if (playerCollider.TryGetComponent<PlayerHealth>(out var health))
                {
                    health.TakeDamage(attackDamage);
                    hitConfirmed = true;
                }

                /* Apply knockback
                if (playerCollider.TryGetComponent<IKnockbackable>(out var knockback))
                {
                    knockback.ApplyKnockback(chargeDirection, attackKnockback);
                }
                */
            }
        }

        // Visual feedback
        if (hitConfirmed)
        {
            if (hitEffectPrefab) Instantiate(hitEffectPrefab, attackPos, Quaternion.identity);
            StartCoroutine(HitFreezeEffect());
        }
    }

    private IEnumerator HitFreezeEffect()
    {
        if (hitFreezeDuration > 0)
        {
            Time.timeScale = 0f;
            yield return new WaitForSecondsRealtime(hitFreezeDuration);
            Time.timeScale = 1f;
        }
    }

    public void ChasePlayer(float distanceToPlayer)
    {
        if (distanceToPlayer > stoppingDistance)
        {
            Vector2 direction = (player.position - transform.position).normalized;
            rb.linearVelocity = new Vector2(direction.x * chaseSpeed, rb.linearVelocity.y);
            
            // Flip based on movement direction
            transform.localScale = new Vector3(
                direction.x > 0 ? -1 : 1, 
                1, 
                1);
        }
        else
        {
            rb.linearVelocity = Vector2.zero;
        }
    }

    public void Idle()
    {
        rb.linearVelocity = Vector2.zero;
        isChargingAttack = false;
    }

    public void UpdateAnimations()
    {
        bool isMoving = Mathf.Abs(rb.linearVelocity.x) > 0.1f;
        bool isAttacking = Time.time < lastAttackTime + 0.5f;
        
        anim.SetBool("IsMoving", isMoving && !isChargingAttack);
        anim.SetBool("IsAttacking", isAttacking);
        anim.SetBool("IsCharging", isChargingAttack);
    }

    public void OnDrawGizmosSelected()
    {
        // Detection range
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        // Rear detection range
        Gizmos.color = new Color(1f, 0.5f, 0f);
        Gizmos.DrawWireSphere(transform.position, detectionRange * rearDetectionReduction);

        // Attack range
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        // Attack area (now shows direction)
        Gizmos.color = Color.magenta;
        Vector2 attackPos = (Vector2)transform.position + (chargeDirection != Vector2.zero ? chargeDirection : Vector2.right) * (attackSize.x * 0.6f);
        Gizmos.DrawWireCube(attackPos, attackSize);
        
        // Attack direction indicator
        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(transform.position, attackPos);
    }
}
