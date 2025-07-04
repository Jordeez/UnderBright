using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Animator))]
public class EnemyChase_Attack : MonoBehaviour
{
    [Header("Detection Settings")]
    [SerializeField] private float detectionRange = 5f;
    [SerializeField] private float attackRange = 1f;
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private float rearDetectionMultiplier = 0.5f;

    [Header("Movement Settings")]
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
    private Transform player;
    private EnemyPatrol patrolController;

    // State variables
    private float lastAttackTime;
    private bool facingRight = true;
    private bool isChargingAttack;
    private float chargeStartTime;
    private float currentDetectionRange;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        patrolController = GetComponent<EnemyPatrol>();
        player = GameObject.FindWithTag("Player")?.transform;
    }

    private void Update()
    {
        if (player == null) return;

        UpdateDetectionState();
        HandleEnemyBehavior();
        UpdateAnimations();
    }

    private void UpdateDetectionState()
    {
        Vector2 toPlayer = player.position - transform.position;
        float dotProduct = Vector2.Dot(toPlayer.normalized, facingRight ? Vector2.right : Vector2.left);
        currentDetectionRange = dotProduct < 0 ? detectionRange * rearDetectionMultiplier : detectionRange;
    }

    private void HandleEnemyBehavior()
    {
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if (distanceToPlayer <= currentDetectionRange)
        {
            patrolController?.SetPatrolling(false);

            if (ShouldStartAttack(distanceToPlayer))
            {
                StartCharge();
            }
            else if (isChargingAttack)
            {
                ChargeAttack();
            }
            else
            {
                ChasePlayer(distanceToPlayer);
            }
        }
        else
        {
            patrolController?.SetPatrolling(true);
            Idle();
            isChargingAttack = false;
        }
    }

    private bool ShouldStartAttack(float distanceToPlayer)
    {
        return !isChargingAttack && 
               distanceToPlayer <= attackRange && 
               Time.time > lastAttackTime + attackCooldown;
    }

    private void StartCharge()
    {
        isChargingAttack = true;
        chargeStartTime = Time.time;
        rb.velocity = Vector2.zero;
    }

    private void ChargeAttack()
    {
        if (Time.time >= chargeStartTime + chargeTime)
        {
            Attack();
            isChargingAttack = false;
        }
    }

    private void Attack()
    {
        lastAttackTime = Time.time;
        
        Collider2D[] hitPlayers = Physics2D.OverlapBoxAll(
            transform.position, 
            attackSize, 
            0f, 
            playerLayer);

        foreach (Collider2D playerCollider in hitPlayers)
        {
            playerCollider.GetComponent<PlayerHealth>()?.TakeDamage(attackDamage);
        }
    }

    private void ChasePlayer(float distanceToPlayer)
    {
        if (distanceToPlayer > stoppingDistance)
        {
            Vector2 direction = (player.position - transform.position).normalized;
            rb.velocity = new Vector2(direction.x * chaseSpeed, rb.velocity.y);
            FlipSprite(direction.x);
        }
        else
        {
            rb.velocity = new Vector2(0, rb.velocity.y);
        }
    }

    private void Idle()
    {
        rb.velocity = new Vector2(0, rb.velocity.y);
    }

    private void UpdateAnimations()
    {
        bool isMoving = Mathf.Abs(rb.velocity.x) > 0.1f && !isChargingAttack;
        bool isAttacking = Time.time < lastAttackTime + 0.5f;
        bool isCharging = isChargingAttack && Time.time < chargeStartTime + chargeTime;
        
        anim.SetBool("IsMoving", isMoving);
        anim.SetBool("IsAttacking", isAttacking);
        anim.SetBool("IsCharging", isCharging);
    }

    private void FlipSprite(float directionX)
    {
        if ((directionX > 0 && !facingRight) || (directionX < 0 && facingRight))
        {
            facingRight = !facingRight;
            Vector3 scale = transform.localScale;
            scale.x *= -1;
            transform.localScale = scale;
        }
    }

    private void OnDrawGizmosSelected()
    {
        // Detection range
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        // Forward detection area
        Gizmos.color = new Color(1f, 0.5f, 0f);
        Vector3 forwardCenter = transform.position + (facingRight ? Vector3.right : Vector3.left) * detectionRange * 0.25f;
        Gizmos.DrawWireSphere(forwardCenter, detectionRange * 0.5f);

        // Attack range
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        // Attack area
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireCube(transform.position, attackSize);
    }
}
