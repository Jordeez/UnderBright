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
    [SerializeField] [Range(0, 1)] private float rearDetectionReduction = 0.5f;

    [Header("Chase Settings")]
    [SerializeField] private float chaseSpeed = 3f;
    [SerializeField] private float stoppingDistance = 0.5f;

    [Header("Charge Attack Settings")]
    [SerializeField] private float attackCooldown = 2f;
    [SerializeField] private float chargeTime = 0.5f; // Wind-up before attacking
    [SerializeField] private float chargeSpeedMultiplier = 1.5f; // Faster dash during charge
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
    private Vector2 chargeDirection; // Stores direction during charge

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
        if (player == null) return false;
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
        else
        {
            // Continue charging toward the player
            rb.velocity = chargeDirection * (chaseSpeed * chargeSpeedMultiplier);
        }
    }

    private void StartCharge()
    {
        isChargingAttack = true;
        chargeStartTime = Time.time;
        chargeDirection = (player.position - transform.position).normalized;
        anim.SetTrigger("StartCharge"); // Optional: Add a charge animation
    }

    private void ExecuteAttack()
    {
        isChargingAttack = false;
        lastAttackTime = Time.time;

        // Detect hits in front of the enemy
        Vector2 attackCenter = (Vector2)transform.position + chargeDirection * (attackSize.x / 2);
        Collider2D[] hitPlayers = Physics2D.OverlapBoxAll(attackCenter, attackSize, 0, playerLayer);

        foreach (Collider2D playerCollider in hitPlayers)
        {
            playerCollider.GetComponent<PlayerHealth>()?.TakeDamage(attackDamage);
        }

        rb.velocity = Vector2.zero; // Stop after attack
    }

    private void ChasePlayer(float distanceToPlayer)
    {
        if (distanceToPlayer > stoppingDistance)
        {
            Vector2 direction = (player.position - transform.position).normalized;
            rb.velocity = new Vector2(direction.x * chaseSpeed, rb.velocity.y);

            // Flip sprite based on movement direction
            if (Mathf.Abs(direction.x) > 0.1f)
            {
                transform.localScale = new Vector3(
                    direction.x > 0 ? -1 : 1, 
                    1, 
                    1);
            }
        }
        else
        {
            rb.velocity = new Vector2(0, rb.velocity.y);
        }
    }

    private void Idle()
    {
        rb.velocity = Vector2.zero;
        isChargingAttack = false;
    }

    private void UpdateAnimations()
    {
        anim.SetBool("IsMoving", Mathf.Abs(rb.velocity.x) > 0.1f && !isChargingAttack);
        anim.SetBool("IsCharging", isChargingAttack);
    }

    private void OnDrawGizmosSelected()
    {
        // Detection range
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        // Attack range
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        // Charge attack area
        if (isChargingAttack)
        {
            Gizmos.color = Color.magenta;
            Vector2 attackCenter = (Vector2)transform.position + chargeDirection * (attackSize.x / 2);
            Gizmos.DrawWireCube(attackCenter, attackSize);
        }
    }
}
