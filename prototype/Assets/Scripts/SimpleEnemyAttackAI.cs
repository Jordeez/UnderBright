using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    [Header("Attack Settings")]
    [SerializeField] private float attackRange = 1.5f; // Range at which enemy can attack
    [SerializeField] private int attackDamage = 1; // Damage dealt per attack
    [SerializeField] private float attackCooldown = 2f; // Time between attacks
    [SerializeField] private LayerMask playerLayer; // Layer for the player

    [Header("References")]
    [SerializeField] private Transform attackPoint; // Point from which attack is detected
    [SerializeField] private Animator animator; // Reference to animator (optional)

    private float cooldownTimer = 0f; // Timer for attack cooldown
    private bool playerInRange = false; // Track if player is in range

    private void Update()
    {
        // Update cooldown timer
        if (cooldownTimer > 0)
        {
            cooldownTimer -= Time.deltaTime;
        }

        // Check for player in range
        playerInRange = Physics2D.OverlapCircle(attackPoint.position, attackRange, playerLayer);

        // Attack if player is in range and cooldown is finished
        if (playerInRange && cooldownTimer <= 0)
        {
            Attack();
            cooldownTimer = attackCooldown;
        }
    }

    private void Attack()
    {
        // Play attack animation if animator exists
        if (animator != null)
        {
            animator.SetTrigger("Attack");
        }

        // Detect player in attack range
        Collider2D[] hitPlayers = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, playerLayer);

        // Damage all players hit
        foreach (Collider2D player in hitPlayers)
        {
            // Assuming player has a PlayerHealth component with TakeDamage method
            PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(attackDamage);
            }
        }
    }

    // Visualize attack range in editor
    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;
        
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}
