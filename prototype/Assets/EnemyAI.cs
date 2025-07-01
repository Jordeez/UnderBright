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
        GetComponent<BoxCollider2D>().isTrigger() = true;


        //Setting The Enemy Root Object (Position)
        GameObject root = new GameObject(name + "_Root");
        //Reset the position of the enemy
        root.transform.position = transform.position;
        //This sets the enemy child node
        transform.SetParent();
        //This Create The Waypoints between the 2 platform
        GameObject waypoints = new GameObejct("Waypoint");
        //Creates The enemy object
        waypoints.transform.SetParent(root.transform);
        waypoints.transform.position = root.transform.position;
        //Make Waypoints of the child nodes(Enemy)
        //Basically This sets the global position of the boss
        GameObject p1 = new GameObject("Point1"); p1.transform.SetParent(waypoints.transform); p1.transform.position = root.transform.position;
        GameObject p2 = new GameObject("Point2"); p2.transform.SetParent(waypoints.transform); p2.transform.position = root.transform.position;

        //Initialize The Points List
        points = List<Transform>();
        points.Add(p1.transform);
    }

    private void Update()
    {
        MoveToNextPoint();
    }

    //We're making the enemy patrol system
    public void MoveToNextPoint()
    {
        //Basically, This gets the enemy ai to the next point
        //Then, We Flip The Enemy Character (With Animation) to the Golden Point or P2
        Transform goalPoint = points[nextId];
        //the next point of the enemy ai
        if (goalPoint.transform.position.x > transform.position.x)
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }
        else
        {
            transform.localScale = new Vector3(1, 1, 1);
        }
        //Moving The Enemy Towards The Goal Point
        transform.position = Vector2.MoveTowards(transform.position, goalPoint.position, speed * Time.deltaTime);
        //Checking The Distance Between The 2 Points of The AI (To Trigger The Next Point)
        if (Vector2.Distance(transform.position, goalPoint.position) < 0.5f)
        {
            //This Checks if we are at the end of the line (position -= 1)
            //Also Checks if we are at the next point (position += 1)
            //After The Condition Checking we Apply changes to the 'nextID' variable
            if (nextId == points.Count - 1)
            {
                changeValueID = -1;
            }
            if (nextId == 0)
            {
                changeValueID = 1;
            } nextId += changeValueID;

        }

    }

}