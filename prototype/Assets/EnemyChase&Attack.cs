using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chase_Attack : MonoBehaviour
{
    public float distance;
    public Transform Player;
    public UnityEngine.AI.NavMeshAgent navMeshAgent;

    private Animator anim;

    void Start()
    {
        anim = GetComponent<Animator>();
        Update();
    }

    void Update()
    {
        distance = Vector3.Distance(this.transform.position, Player.position); //gets the players position

        if (distance < 10)
        {
            navMeshAgent.destination = Player.position; //depends on what you set
        }

        if (navMeshAgent.velocity.magnitude > 1)
        {
            anim.SetInteger("Mode", 1); //chases the player (animation)
        }
        else
        {
            if (distance > 5)
            {
                anim.SetInteger("Mode", 0); //enemy stays dormant after the player is chased
            }
            else
            {
                anim.SetInteger("Mode", 2); //enemy attacks the player
            }
        }
    }
}