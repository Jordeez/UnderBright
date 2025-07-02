using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent), typeof(Animator))]
public class EnemyChase_Attack : MonoBehaviour
{
    public float distance;
    public Transform Player;
    private NavMeshAgent navMeshAgent;
    private Animator anim;

    void Awake()
    {
        // Automatically fetch required components
        navMeshAgent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        distance = Vector3.Distance(transform.position, Player.position);

        if (distance < 10f)
        {
            navMeshAgent.destination = Player.position;
        }

        if (navMeshAgent.velocity.magnitude > 1f)
        {
            anim.SetInteger("Mode", 1); // chasing
        }
        else
        {
            if (distance > 5f)
            {
                anim.SetInteger("Mode", 0); // idle
            }
            else
            {
                anim.SetInteger("Mode", 2); // attacking
            }
        }
    }
}
