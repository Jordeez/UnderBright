using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(Rigidbody2D), typeof(Animator), typeof(EnemyAI))]
public class EnemyChase_Attack : MonoBehaviour
{
    [Header("Detection")]
    [SerializeField] private float detectionRange = 5f;
    [SerializeField] private float attackRange = 1.5f;
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private float visionConeAngle = 90f; // Wider cone for platformers

    [Header("Movement")]
    [SerializeField] private float chaseSpeed = 4f;
    [SerializeField] private float stoppingDistance = 1f;
    [SerializeField] private float ledgeCheckDistance = 2f; // Stops at edges
    [SerializeField] private LayerMask groundLayer;

    [Header("Jumping")]
    [SerializeField] private float jumpForce = 8f;
    [SerializeField] private float jumpCooldown = 2f;
    [SerializeField] private float minJumpHeightDiff = 1f; // Jump if player is higher

    [Header("Attack")]
    [SerializeField] private float attackCooldown = 1.5f;
    [SerializeField] private int attackDamage = 1;
    [SerializeField] private Vector2 attackSize = new Vector2(1.5f, 1f);
    [SerializeField] private float attackKnockback = 5f;

    // Components
    private Rigidbody2D rb;
    private Animator anim;
    private EnemyAI enemyAI;
    private Transform player;
    private PlayerHealth playerHealth;

    // State
    private float lastAttackTime;
    private float lastJumpTime;
    private bool isFacingRight = true;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        enemyAI = GetComponent<EnemyAI>();
        FindPlayer();
    }

    private void FindPlayer()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        if (player != null) playerHealth = player.GetComponent<PlayerHealth>();
    }

    private void Update()
    {
        if (player == null) return;

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);
        bool canSeePlayer = CanDetectPlayer(distanceToPlayer);

        if (canSeePlayer)
        {
            HandlePlayerDetected(distanceToPlayer);
            TryJump();
        }
        else
        {
            HandlePlayerLost();
        }

        UpdateAnimations();
    }

    private bool CanDetectPlayer(float distance)
    {
        if (distance > detectionRange) return false;

        Vector2 dirToPlayer = (player.position - transform.position).normalized;
        float angle = Vector2.Angle(isFacingRight ? Vector2.right : Vector2.left, dirToPlayer);
        return angle < visionConeAngle / 2;
    }

    private void HandlePlayerDetected(float distance)
    {
        if (enemyAI.enabled)
        {
            enemyAI.enabled = false;
            rb.velocity = Vector2.zero;
        }

        FlipTowardsPlayer();

        if (distance <= attackRange && Time.time > lastAttackTime + attackCooldown)
        {
            Attack();
        }
        else if (distance > stoppingDistance)
        {
            ChasePlayer();
        }
        else
        {
            rb.velocity = new Vector2(0, rb.velocity.y);
        }
    }

    private void FlipTowardsPlayer()
    {
        bool playerIsRight = player.position.x > transform.position.x;
        if (playerIsRight != isFacingRight)
        {
            isFacingRight = playerIsRight;
            transform.localScale = new Vector3(
                isFacingRight ? 1 : -1, 
                transform.localScale.y, 
                transform.localScale.z);
        }
    }

    private void ChasePlayer()
    {
        if (IsNearLedge()) return; // Don't walk off ledges

        float direction = isFacingRight ? 1 : -1;
        rb.velocity = new Vector2(direction * chaseSpeed, rb.velocity.y);
    }

    private bool IsNearLedge()
    {
        Vector2 checkPos = transform.position + 
            (isFacingRight ? Vector3.right : Vector3.left) * 0.5f;
        return !Physics2D.Raycast(checkPos, Vector2.down, ledgeCheckDistance, groundLayer);
    }

    private void TryJump()
    {
        if (Time.time < lastJumpTime + jumpCooldown) return;
        if (!IsGrounded()) return;

        // Jump if player is significantly higher
        if (player.position.y > transform.position.y + minJumpHeightDiff)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            lastJumpTime = Time.time;
        }
    }

    private bool IsGrounded()
    {
        return Physics2D.Raycast(transform.position, Vector2.down, 0.1f, groundLayer);
    }

    private void Attack()
    {
        lastAttackTime = Time.time;
        anim.SetTrigger("Attack");

        // Detect hit after animation delay (use Animation Events in practice)
        Collider2D[] hits = Physics2D.OverlapBoxAll(
            transform.position + (isFacingRight ? Vector3.right : Vector3.left) * attackSize.x / 2,
            attackSize, 
            0, 
            playerLayer);

        foreach (var hit in hits)
        {
            playerHealth?.TakeDamage(attackDamage);
            hit.attachedRigidbody?.AddForce(
                (hit.transform.position - transform.position).normalized * attackKnockback, 
                ForceMode2D.Impulse);
        }
    }

    private void HandlePlayerLost()
    {
        if (!enemyAI.enabled)
        {
            enemyAI.enabled = true;
            rb.velocity = Vector2.zero;
        }
    }

    private void UpdateAnimations()
    {
        anim.SetBool("IsMoving", Mathf.Abs(rb.velocity.x) > 0.1f);
        anim.SetBool("IsGrounded", IsGrounded());
        anim.SetFloat("VerticalVelocity", rb.velocity.y);
    }

    private void OnDrawGizmosSelected()
    {
        // Detection
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        // Attack
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(
            transform.position + (isFacingRight ? Vector3.right : Vector3.left) * attackSize.x / 2,
            attackSize);

        // Ledge check
        Gizmos.color = Color.blue;
        Vector3 ledgePos = transform.position + 
            (isFacingRight ? Vector3.right : Vector3.left) * 0.5f;
        Gizmos.DrawLine(ledgePos, ledgePos + Vector3.down * ledgeCheckDistance);
    }
}
