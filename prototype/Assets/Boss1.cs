// using UnityEngine;
// using System.Collections;
// using System.Collections.Generic;

// public class WitchBoss : MonoBehaviour
// {
//     [Header("Movement")]
//     public float moveSpeed = 3f;
//     public Transform[] patrolPoints;
//     private int currentPatrolIndex = 0;
//     private bool movingRight = true;
//     private Rigidbody2D rb;

//     [Header("Attack Settings")]
//     public float attackCooldown = 2f;
//     public GameObject projectilePrefab;
//     public Transform projectileSpawnPoint;
//     public float projectileSpeed = 8f;
//     public float summonCooldown = 5f;
    
//     [Header("Minion Summoning")]
//     public GameObject minionPrefab;
//     public Transform[] summonPoints;
//     public int minionsPerSummon = 3;

//     [Header("Health")]
//     public int maxHealth = 100;
//     private int currentHealth;
//     public HealthBar healthBar;

//     [Header("Phases")]
//     public Phase[] phases;
//     private int currentPhaseIndex = 0;
    
//     [System.Serializable]
//     public class Phase
//     {
//         public int healthThreshold;
//         public float attackRateMultiplier = 1f;
//         public float moveSpeedMultiplier = 1f;
//         public int additionalMinions = 0;
//     }

//     private bool isAttacking = false;
//     private float nextAttackTime = 0f;
//     private float nextSummonTime = 0f;

//     private Animator animator;
//     private SpriteRenderer spriteRenderer;

//     void Start()
//     {
//         rb = GetComponent<Rigidbody2D>();
//         animator = GetComponent<Animator>();
//         spriteRenderer = GetComponent<SpriteRenderer>();
//         currentHealth = maxHealth;
        
//         if (healthBar != null)
//         {
//             healthBar.SetMaxHealth(maxHealth);
//         }

//         if (phases.Length == 0)
//         {
//             Debug.LogError("No phases set up for boss!");
//         }
//     }

//     void Update()
//     {
//         HandleMovement();
//         CheckPhaseChange();
        
//         if (Time.time >= nextAttackTime && !isAttacking)
//         {
//             StartCoroutine(AttackRoutine());
//             nextAttackTime = Time.time + attackCooldown * phases[currentPhaseIndex].attackRateMultiplier;
//         }
        
//         if (Time.time >= nextSummonTime)
//         {
//             SummonMinions();
//             nextSummonTime = Time.time + summonCooldown;
//         }
//     }

//     void HandleMovement()
//     {
//         if (patrolPoints.Length < 2) return;

//         float phaseSpeed = moveSpeed * phases[currentPhaseIndex].moveSpeedMultiplier;
        
//         // Move towards current patrol point
//         Vector2 targetPosition = patrolPoints[currentPatrolIndex].position;
//         transform.position = Vector2.MoveTowards(transform.position, targetPosition, phaseSpeed * Time.deltaTime);

//         // Flip sprite based on movement direction
//         if ((targetPosition - (Vector2)transform.position).x > 0.1f)
//         {
//             spriteRenderer.flipX = false;
//         }
//         else if ((targetPosition - (Vector2)transform.position).x < -0.1f)
//         {
//             spriteRenderer.flipX = true;
//         }

//         // Check if reached patrol point
//         if (Vector2.Distance(transform.position, targetPosition) < 0.1f)
//         {
//             currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Length;
//         }
//     }

//     IEnumerator AttackRoutine()
//     {
//         isAttacking = true;
//         animator.SetTrigger("Attack");

//         // Wait for animation to reach the attack frame
//         yield return new WaitForSeconds(0.3f);
        
//         // Fire projectile
//         GameObject projectile = Instantiate(projectilePrefab, projectileSpawnPoint.position, Quaternion.identity);
//         Vector2 direction = (spriteRenderer.flipX ? Vector2.left : Vector2.right) * projectileSpeed;
//         projectile.GetComponent<Rigidbody2D>().velocity = direction;
        
//         // Flip projectile to match direction
//         projectile.transform.localScale = new Vector3(
//             spriteRenderer.flipX ? -1 : 1,
//             1,
//             1
//         );

//         yield return new WaitForSeconds(0.5f);
//         isAttacking = false;
//     }

//     void SummonMinions()
//     {
//         if (summonPoints.Length == 0 || minionPrefab == null) return;

//         animator.SetTrigger("Summon");

//         int minionsToSummon = minionsPerSummon + phases[currentPhaseIndex].additionalMinions;
//         minionsToSummon = Mathf.Min(minionsToSummon, summonPoints.Length);
        
//         // Shuffle summon points
//         List<Transform> shuffledPoints = new List<Transform>(summonPoints);
//         ShuffleList(shuffledPoints);

//         for (int i = 0; i < minionsToSummon; i++)
//         {
//             Instantiate(minionPrefab, shuffledPoints[i].position, Quaternion.identity);
//         }
//     }

//     void ShuffleList<T>(List<T> list)
//     {
//         for (int i = 0; i < list.Count; i++)
//         {
//             int randomIndex = Random.Range(i, list.Count);
//             T temp = list[randomIndex];
//             list[randomIndex] = list[i];
//             list[i] = temp;
//         }
//     }

//     public void TakeDamage(int damage)
//     {
//         currentHealth -= damage;
//         animator.SetTrigger("Hit");
        
//         if (healthBar != null)
//         {
//             healthBar.SetHealth(currentHealth);
//         }

//         if (currentHealth <= 0)
//         {
//             Die();
//         }
//     }

//     void CheckPhaseChange()
//     {
//         if (currentPhaseIndex < phases.Length - 1 && 
//             currentHealth <= phases[currentPhaseIndex].healthThreshold)
//         {
//             currentPhaseIndex++;
//             animator.SetTrigger("PhaseChange");
            
//             // Phase change effects
//             if (currentPhaseIndex == phases.Length - 1) // Final phase
//             {
//                 attackCooldown *= 0.5f; // Attack twice as fast in final phase
//             }
//         }
//     }

//     void Die()
//     {
//         animator.SetTrigger("Die");
//         // Disable all behaviors
//         enabled = false;
//         GetComponent<Collider2D>().enabled = false;
        
//         // Death effects could be added here
//         Destroy(gameObject, 2f);
//     }

//     // To visualize patrol points in editor
//     void OnDrawGizmosSelected()
//     {
//         if (patrolPoints != null && patrolPoints.Length > 0)
//         {
//             Gizmos.color = Color.yellow;
//             for (int i = 0; i < patrolPoints.Length; i++)
//             {
//                 if (i < patrolPoints.Length - 1)
//                 {
//                     Gizmos.DrawLine(patrolPoints[i].position, patrolPoints[i + 1].position);
//                 }
//                 else
//                 {
//                     Gizmos.DrawLine(patrolPoints[i].position, patrolPoints[0].position);
//                 }
                
//                 Gizmos.DrawWireSphere(patrolPoints[i].position, 0.3f);
//             }
//         }
//     }
// }
