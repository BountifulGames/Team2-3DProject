using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class TestNavAgent : MonoBehaviour
{
    public Transform[] waypoints; // Assign this in the Inspector
    private NavMeshAgent agent;
    private int currentWaypointIndex;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        currentWaypointIndex = 0;
        agent.destination = waypoints[currentWaypointIndex].position;
    }

    void Update()
    {
        if (Vector3.Distance(transform.position, waypoints[currentWaypointIndex].position) <= agent.stoppingDistance)
        {
            currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length;
            agent.destination = waypoints[currentWaypointIndex].position;
        }
    }
}
