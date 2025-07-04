using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyChase_Attack : MonoBehaviour
{
    [Header("Detection")]
    [SerializeField] private float detectionRange = 5f;
    [SerializeField] private float attackRange = 1f;
    [SerializeField] private LayerMask playerLayer;

    [Header("Movement")]
    [SerializeField] private float chaseSpeed = 3f;
    [SerializeField] private float stoppingDistance = 0.5f;
    
    [Header("Attack")]
    [SerializeField] private float attackCooldown = 2f;
    [SerializeField] private float chargeTime = 0.5f; // Time to charge before attacking
    [SerializeField] private int attackDamage = 1;
    [SerializeField] private Vector2 attackSize = new Vector2(1f, 1f);

    private Transform player;
    private Rigidbody2D rb;
    private Animator anim;
    private float lastAttackTime;
    private bool facingRight = true;
    private bool isChargingAttack = false;
    private float chargeStartTime;
    private float forwardDetectionRange; // Detection range in facing direction

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        forwardDetectionRange = detectionRange;
    }

    private void Update()
    {
        if (player == null) return;

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);
        
        // Adjust detection range based on facing direction
        UpdateDirectionalDetection();

        // Detection
        if (distanceToPlayer <= forwardDetectionRange)
        {
            if (isChargingAttack)
            {
                ChargeAttack();
            }
            else if (distanceToPlayer <= attackRange && Time.time > lastAttackTime + attackCooldown)
            {
                StartCharge();
            }
            else // Chase if not attacking or charging
            {
                ChasePlayer();
            }
        }
        else // Idle if player not detected
        {
            Idle();
            isChargingAttack = false; // Cancel charge if player leaves detection
        }

        UpdateAnimations(distanceToPlayer);
        FlipSprite();
    }

    private void UpdateDirectionalDetection()
    {
        // Halve the detection range behind the enemy
        Vector2 toPlayer = player.position - transform.position;
        float dotProduct = Vector2.Dot(toPlayer.normalized, facingRight ? Vector2.right : Vector2.left);
        
        // If player is behind the enemy (dot product < 0), use half detection range
        forwardDetectionRange = dotProduct < 0 ? detectionRange * 0.5f : detectionRange;
    }

    private void StartCharge()
    {
        isChargingAttack = true;
        chargeStartTime = Time.time;
        rb.velocity = Vector2.zero;
    }

    private void ChargeAttack()
    {
        // Continue charging until charge time is complete
        if (Time.time < chargeStartTime + chargeTime)
        {
            // You could add visual/audio feedback here for charging
            return;
        }

        // Charge complete, perform attack
        Attack();
        isChargingAttack = false;
    }

    private void ChasePlayer()
    {
        Vector2 direction = (player.position - transform.position).normalized;
        
        // Only move if outside stopping distance
        if (Vector2.Distance(transform.position, player.position) > stoppingDistance)
        {
            rb.velocity = new Vector2(direction.x * chaseSpeed, rb.velocity.y);
        }
        else
        {
            rb.velocity = new Vector2(0, rb.velocity.y);
        }
    }

    private void Attack()
    {
        lastAttackTime = Time.time;
        
        // Check for player in attack range
        Collider2D[] hitPlayers = Physics2D.OverlapBoxAll(
            transform.position, 
            attackSize, 
            0f, 
            playerLayer);

        foreach (Collider2D player in hitPlayers)
        {
            player.GetComponent<PlayerHealth>().TakeDamage(attackDamage);
        }
    }

    private void Idle()
    {
        rb.velocity = new Vector2(0, rb.velocity.y);
    }

    private void UpdateAnimations(float distanceToPlayer)
    {
        bool isMoving = Mathf.Abs(rb.velocity.x) > 0.1f && !isChargingAttack;
        bool isAttacking = Time.time < lastAttackTime + 0.5f; // Attack animation duration
        bool isCharging = isChargingAttack && Time.time < chargeStartTime + chargeTime;
        
        anim.SetBool("IsMoving", isMoving);
        anim.SetBool("IsAttacking", isAttacking && distanceToPlayer <= attackRange);
        anim.SetBool("IsCharging", isCharging);
    }

    private void FlipSprite()
    {
        if (player.position.x > transform.position.x && !facingRight)
        {
            Flip();
        }
        else if (player.position.x < transform.position.x && facingRight)
        {
            Flip();
        }
    }

    private void Flip()
    {
        facingRight = !facingRight;
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }

    // Visualize ranges in editor
    private void OnDrawGizmosSelected()
    {
        // Full detection range
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
        
        
        Gizmos.color = new Color(1f, 0.5f, 0f); // Orange color for forward detection
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
