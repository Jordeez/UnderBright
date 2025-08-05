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
    [SerializeField, Range(0, 1)] private float rearDetectionReduction = 0.5f;

    [Header("Chase Settings")]
    [SerializeField] private float chaseSpeed = 3f;
    [SerializeField] private float stoppingDistance = 0.5f;

    [Header("Attack Settings")]
    [SerializeField] private float attackCooldown = 2f;
    [SerializeField] private float chargeTime = 0.5f;
    [SerializeField] private int attackDamage = 1;
    [SerializeField] private Vector2 attackSize = new Vector2(1.5f, 1.5f);
    [SerializeField] private float attackKnockback = 5f;
    [SerializeField] private float selfKnockback = 3f;
    [SerializeField] private float hitFreezeDuration = 0.1f;
    [SerializeField] private GameObject hitEffectPrefab;

    private Rigidbody2D rb;
    private Animator anim;
    private EnemyAI enemyAI;
    private Transform player;

    private float lastAttackTime;
    private bool isChargingAttack;
    private float chargeStartTime;
    private Vector2 chargeDirection;
    private Vector2 targetAttackPosition;
    private bool wasPatrolling;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        enemyAI = GetComponent<EnemyAI>();
        player = GameObject.FindWithTag("Player")?.transform;

        // Disable physical push
        rb.mass = 999f; // or set constraints as well
        rb.sharedMaterial = new PhysicsMaterial2D { friction = 0, bounciness = 0 };
    }

    private void Update()
    {
        if (player == null) return;

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);
        bool playerInFront = IsPlayerInFront();
        float effectiveDetectionRange = playerInFront ? detectionRange : detectionRange * rearDetectionReduction;

        if (isChargingAttack)
        {
            // Stop on wall
            RaycastHit2D hit = Physics2D.Raycast(transform.position, chargeDirection, 0.5f, LayerMask.GetMask("Ground"));
            if (hit.collider != null)
            {
                StopDash();
                return;
            }

            // Reached dash duration
            if (Time.time >= chargeStartTime + chargeTime)
            {
                StopDash();
                return;
            }

            rb.linearVelocity = chargeDirection * chaseSpeed * 2f;
        }

        if (distanceToPlayer <= effectiveDetectionRange)
        {
            anim.SetBool("isHostile", true);
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
        else
        {
            CheckHitDuringDash();
        }
    }

    private void StartCharge()
    {
        isChargingAttack = true;
        chargeStartTime = Time.time;

        // Lock in the player's last known position
        targetAttackPosition = player.position;
        Vector2 direction = (targetAttackPosition - (Vector2)transform.position).normalized;
        chargeDirection = new Vector2(Mathf.Sign(direction.x), 0f); // left/right only

        rb.linearVelocity = chargeDirection * chaseSpeed * 2f;
        anim.SetTrigger("StartCharge");
    }

    private void CheckHitDuringDash()
    {
        Vector2 attackPos = (Vector2)transform.position + chargeDirection * (attackSize.x * 0.6f);

        Collider2D[] hitPlayers = Physics2D.OverlapBoxAll(
            attackPos, attackSize, 0f, playerLayer);

        foreach (Collider2D col in hitPlayers)
        {
            if (col.CompareTag("Player"))
            {
                if (col.TryGetComponent<PlayerHealth>(out var health))
                {
                    health.TakeDamage(attackDamage);
                }

                if (hitEffectPrefab)
                    Instantiate(hitEffectPrefab, attackPos, Quaternion.identity);

                StartCoroutine(HitFreezeEffect());
                ApplySelfKnockback();

                StopDash();
                break;
            }
        }
    }

    private void ApplySelfKnockback()
    {
        rb.linearVelocity = -chargeDirection * selfKnockback;
    }

    private void StopDash()
    {
        isChargingAttack = false;
        rb.linearVelocity = Vector2.zero;
        lastAttackTime = Time.time;
        anim.SetTrigger("ExecuteAttack");
    }

    private IEnumerator HitFreezeEffect()
    {
        Time.timeScale = 0f;
        yield return new WaitForSecondsRealtime(hitFreezeDuration);
        yield return new WaitForSecondsRealtime(hitFreezeDuration);
        Time.timeScale = 1f;
    }

    private void ChasePlayer(float distanceToPlayer)
    {

        if (distanceToPlayer > stoppingDistance)
        {
            Vector2 direction = (player.position - transform.position).normalized;
            rb.linearVelocity = new Vector2(direction.x * chaseSpeed, rb.linearVelocity.y);

            transform.localScale = new Vector3(
                direction.x > 0 ? -0.5f : 0.5f,
                0.5f,
                1);
        }
        else
        {
            rb.linearVelocity = Vector2.zero;
        }
    }

    private void Idle()
    {
        anim.SetBool("isHostile", false);
        rb.linearVelocity = Vector2.zero;
        isChargingAttack = false;
    }

    private void UpdateAnimations()
    {
        bool isMoving = Mathf.Abs(rb.linearVelocity.x) > 0.1f;
        bool isAttacking = Time.time < lastAttackTime + 0.5f;

        /*
        anim.SetBool("IsMoving", isMoving && !isChargingAttack);
        anim.SetBool("IsAttacking", isAttacking);
        anim.SetBool("IsCharging", isChargingAttack);
        */
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        Gizmos.color = new Color(1f, 0.5f, 0f);
        Gizmos.DrawWireSphere(transform.position, detectionRange * rearDetectionReduction);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        Gizmos.color = Color.magenta;
        Vector2 attackPos = (Vector2)transform.position + (chargeDirection != Vector2.zero ? chargeDirection : Vector2.right) * (attackSize.x * 0.6f);
        Gizmos.DrawWireCube(attackPos, attackSize);

        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(transform.position, attackPos);
    }
}
