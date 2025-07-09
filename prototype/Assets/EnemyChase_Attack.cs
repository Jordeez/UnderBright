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
    [SerializeField] private Vector2 attackSize = new Vector2(1f, 1f);

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

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        enemyAI = GetComponent<EnemyAI>();
        player = GameObject.FindWithTag("Player")?.transform;
    }

    private void Update()
    {
        if (player == null) return;

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);
        bool playerInFront = IsPlayerInFront();

        float effectiveDetectionRange = playerInFront ? detectionRange : detectionRange * rearDetectionReduction;

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

    private bool IsPlayerInFront()
    {
        Vector2 toPlayer = (player.position - transform.position).normalized;
        return Vector2.Dot(toPlayer, transform.right) > 0;
    }

    private bool ShouldAttack(float distanceToPlayer)
    {
        return distanceToPlayer <= attackRange && 
               Time.time > lastAttackTime + attackCooldown;
    }

    private void HandleAttack()
    {
        if (!isChargingAttack)
        {
            StartCharge();
        }
        else if (Time.time >= chargeStartTime + chargeTime)
        {
            ExecuteAttack();
        }
    }

    private void StartCharge()
    {
        isChargingAttack = true;
        chargeStartTime = Time.time;
        rb.linearVelocity = Vector2.zero;
    }

    private void ExecuteAttack()
    {
        lastAttackTime = Time.time;
        isChargingAttack = false;

        Collider2D[] hitPlayers = Physics2D.OverlapBoxAll(
            transform.position, 
            attackSize, 
            0f, 
            playerLayer);

        foreach (Collider2D playerCollider in hitPlayers)
        {
            playerCollider.GetComponent<PlayerHealth>()?.TakeDamage(attackDamage);
            playerCollider.GetComponent<KnockbackHandler>()?.ReceiveHit(transform.position);
        }

    }

    private void ChasePlayer(float distanceToPlayer)
    {
        if (distanceToPlayer > stoppingDistance)
        {
            Vector2 direction = (player.position - transform.position).normalized;
            rb.linearVelocity = new Vector2(direction.x * chaseSpeed, rb.linearVelocity.y);
            
            // Flip based on movement direction
            transform.localScale = new Vector3(
                direction.x > 0 ? -15 : 15, 
                15, 
                1);
        }
        else
        {
            rb.linearVelocity = Vector2.zero;
        }
    }

    private void Idle()
    {
        rb.linearVelocity = Vector2.zero;
        isChargingAttack = false;
    }

    private void UpdateAnimations()
    {
        bool isMoving = Mathf.Abs(rb.linearVelocity.x) > 0.1f;
        bool isAttacking = Time.time < lastAttackTime + 0.5f;
        
        anim.SetBool("IsMoving", isMoving && !isChargingAttack);
        anim.SetBool("IsAttacking", isAttacking);
        anim.SetBool("IsCharging", isChargingAttack);
    }

    private void OnDrawGizmosSelected()
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

        // Attack area
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireCube(transform.position, attackSize);
    }
}
