//This Script Can either be used for flying creatures or movement type enemy
using System.Collections.Generic;
using System.Collections;
using UnityEngine;


[RequireComponent(typeof(BoxCollider2D))]
public class EnemyAI : MonoBehaviour
{
    //Reference to the Waypoints
    public List<Transform> points;
    public int nextId = 0;
    //Default changing ID value for The enemy id
    public int changeValueID = 1;
    //The Speed Where the enemy is moving
    public int speed = 2;

    private void Reset()
    {
        Init();
    }

    public void Init()
    {
        //When Enemy Hits an object
        GetComponent<BoxCollider2D>().isTrigger = true;

        //Setting The Enemy Root Object (Position)
        GameObject root = new GameObject(name + "_Root");
        //Reset the position of the enemy
        root.transform.position = transform.position;
        //This sets the enemy child node
        transform.SetParent(root.transform);
        //This Create The Waypoints between the 2 platform
        GameObject waypoints = new GameObject("Waypoints");
        //Creates The enemy object
        waypoints.transform.SetParent(root.transform);
        waypoints.transform.position = root.transform.position;
        //Make Waypoints of the child nodes(Enemy)
        //Basically This sets the global position of the boss
        GameObject p1 = new GameObject("Point1"); p1.transform.SetParent(waypoints.transform); p1.transform.position = Vector3.right * 2 + root.transform.position;
        GameObject p2 = new GameObject("Point2"); p2.transform.SetParent(waypoints.transform); p2.transform.position = Vector3.left * 2 + root.transform.position;

        //Initialize The Points List
        points = new List<Transform>();
        points.Add(p1.transform);
        points.Add(p2.transform);
    }

    private void Update()
    {
        MoveToNextPoint();
    }

    public void MoveToNextPoint()
    {
        if (points == null || points.Count == 0)
            return;

        //Get the next waypoint
        Transform goalPoint = points[nextId];
        
        //Flip the enemy based on direction
        if (goalPoint.transform.position.x > transform.position.x)
        {
            transform.localScale = new Vector3(15, 15, 1);
        }
        else
        {
            transform.localScale = new Vector3(-15, 15, 1);
        }
        
        //Move towards the goal point
        transform.position = Vector2.MoveTowards(transform.position, goalPoint.position, speed * Time.deltaTime);
        
        //Check if we've reached the current waypoint
        if (Vector2.Distance(transform.position, goalPoint.position) < 0.5f)
        {
            //Check if we're at the end of the path
            if (nextId == points.Count - 1)
            {
                changeValueID = -1;
            }
            //Check if we're at the start of the path
            else if (nextId == 0)
            {
                changeValueID = 1;
            }
            
            nextId += changeValueID;
        }
    }
}
