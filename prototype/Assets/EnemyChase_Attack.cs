using System.Collections.Generic;
using System.Collections;
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
    [SerializeField] private int attackDamage = 1;
    [SerializeField] private Vector2 attackSize = new Vector2(1f, 1f);

    private Transform player;
    private Rigidbody2D rb;
    private Animator anim;
    private float lastAttackTime;
    private bool facingRight = true;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    private void Update()
    {
        if (player == null) return;

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);
        
        // Detection
        if (distanceToPlayer <= detectionRange)
        {
            // Attack if in range
            if (distanceToPlayer <= attackRange && Time.time > lastAttackTime + attackCooldown)
            {
                Attack();
            }
            else // Chase if not attacking
            {
                ChasePlayer();
            }
        }
        else // Idle if player not detected
        {
            Idle();
        }

        UpdateAnimations(distanceToPlayer);
        FlipSprite();
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
        rb.velocity = Vector2.zero;
        lastAttackTime = Time.time;
        
        // Check for player in attack range
        Collider2D[] hitPlayers = Physics2D.OverlapBoxAll(
            transform.position, 
            attackSize, 
            0f, 
            playerLayer);
        /*
        foreach (Collider2D player in hitPlayers)
        {
            player.GetComponent<PlayerHealth>().TakeDamage(attackDamage);
        }
        */
    }

    private void Idle()
    {
        rb.velocity = new Vector2(0, rb.velocity.y);
    }

    private void UpdateAnimations(float distanceToPlayer)
    {
        bool isMoving = Mathf.Abs(rb.velocity.x) > 0.1f;
        bool isAttacking = Time.time < lastAttackTime + 0.5f; // Attack animation duration
        
        anim.SetBool("isMoving", isMoving);
        anim.SetBool("isAttacking", isAttacking && distanceToPlayer <= attackRange);
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
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
        
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireCube(transform.position, attackSize);
    }
}
