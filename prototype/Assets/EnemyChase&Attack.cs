using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chase_Attack : MonoBehaviour
{
    public float distance;
    public Transform Player;
    public NavMeshAgent navMeshAgent;
    void Start()
    {
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
            Anim.SetInteger("Mode", 1); //chases the player (animation)
        }
        else
        {
            if (distance > 5)
            {
                Anim.SetInteger("Mode", 0); //enemy stays dormant after the player is chased
            }
            else
            {
                Anim.SetInteger("Mode", 2); //enemy attacks the player
            }
        }
    }
}