using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chase_Attack : MonoBehaviour
{
    [Header("Settings")]
    public float chaseRange = 10f;
    public float attackRange = 2f;
    public float attackCooldown = 2f;
    
    [Header("References")]
    public Transform player;
    public NavMeshAgent agent;
    public Animator anim;

    private float lastAttackTime;
    private bool isAttacking;

    private void Start()
    {
        // Auto-get components if not assigned
        if (!agent) agent = GetComponent<NavMeshAgent>();
        if (!anim) anim = GetComponent<Animator>();
        
        // Find player if not assigned
        if (!player) player = GameObject.FindWithTag("Player").transform;
    }

    private void Update()
    {
        if (!player) return;

        float distance = Vector2.Distance(transform.position, player.position);

        // Detection and Chase
        if (distance <= chaseRange)
        {
            // Attack if in range and cooldown finished
            if (distance <= attackRange && Time.time > lastAttackTime + attackCooldown)
            {
                Attack();
            }
            else if (!isAttacking)  // Chase if not attacking
            {
                Chase();
            }
        }
        else
        {
            Idle();
        }
    }

    private void Chase()
    {
        agent.isStopped = false;
        agent.SetDestination(player.position);
        anim.SetInteger("Mode", 1); // Run animation
    }

    private void Attack()
    {
        isAttacking = true;
        agent.isStopped = true;
        anim.SetInteger("Mode", 2); // Attack animation
        
        // Face player during attack
        Vector2 direction = (player.position - transform.position).normalized;
        transform.rotation = Quaternion.LookRotation(direction);
        
        lastAttackTime = Time.time;
        isAttacking = false;
        
        // Add your damage dealing logic here
        Debug.Log("Attacking player!");
    }

    private void Idle()
    {
        agent.isStopped = true;
        anim.SetInteger("Mode", 0); // Idle animation
    }
} //new and improved chasing
