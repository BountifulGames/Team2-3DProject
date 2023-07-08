using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    public float lookRadius = 10f;  // Enemy's field of view radius
    public float fieldOfViewAngle = 120f;  // Angle of enemy's field of view
    public float lostSightDelay = 5f;  // Delay before enemy loses sight of player
    public Transform[] waypoints;  // Array of waypoints for enemy to patrol
    public Transform player;  // Player's position
    public NavMeshAgent agent;  // Enemy's navigation agent
    private float timePlayerOutOfSight;  // Time player is out of enemy's sight
    private int currentWaypointIndex;  // Index of current waypoint in patrol
    
    void Start()
{
    agent = GetComponent<NavMeshAgent>();
    timePlayerOutOfSight = 5f;

    // Reset the enemy's position and start patrolling
    currentWaypointIndex = 0;
    agent.isStopped = false;  // Add this line
    StartCoroutine(Patrol());
}

    void Update()
    {
        float distanceToPlayer = CalculateDistanceToPlayer();

        if (IsPlayerInLookRadius(distanceToPlayer))
        {
            Vector3 directionToPlayer = CalculateDirectionToPlayer();

            if (IsPlayerInViewField(directionToPlayer))
            {
                if (IsLineOfSightClear(directionToPlayer, distanceToPlayer))
                {
                    ResetOutOfSightCounter();
                    PursuePlayer();
                }
                else
                {
                    IncreaseOutOfSightCounter();
                    LoseSightOfPlayerAfterDelay();
                }
            }
            else
            {
                IncreaseOutOfSightCounter();
                LoseSightOfPlayerAfterDelay();
            }
        }
        else
        {
            IncreaseOutOfSightCounter();
            LoseSightOfPlayerAfterDelay();
        }
    }

    float CalculateDistanceToPlayer()
    {
        return Vector3.Distance(player.position, transform.position);
    }

    Vector3 CalculateDirectionToPlayer()
    {
        return (player.position - transform.position).normalized;
    }

    bool IsPlayerInLookRadius(float distance)
    {
        return distance <= lookRadius;
    }

    bool IsPlayerInViewField(Vector3 direction)
    {
        float angle = Vector3.Angle(transform.forward, direction);
        return angle < fieldOfViewAngle / 2f;
    }

    bool IsLineOfSightClear(Vector3 direction, float distance)
    {
        return !Physics.Raycast(transform.position, direction, distance, LayerMask.GetMask("Obstacles"));
    }

    void ResetOutOfSightCounter()
    {
        timePlayerOutOfSight = 0f;
    }

    void IncreaseOutOfSightCounter()
    {
        timePlayerOutOfSight += Time.deltaTime;
    }

    void StopPursuingPlayer()
    {
        if (!IsInvoking("RestartPatrol"))
        {
            Invoke("RestartPatrol", lostSightDelay);
        }
        // animator.SetBool("IsRunning", false);
        // Add your idle or patrol code here
    }

    void PursuePlayer()
    {
        CancelInvoke("RestartPatrol");
        agent.isStopped = false;
        StopCoroutine(Patrol());
        agent.destination = player.position;
        // animator.SetBool("IsRunning", true);
        // Add your attack code here if the player is close enough to the enemy
    }


    void LoseSightOfPlayerAfterDelay()
    {
        if (timePlayerOutOfSight >= lostSightDelay)
        {
            StopPursuingPlayer();
        }
    }

    // Display the lookRadius in editor
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, lookRadius);

        // Draw the field of view
        Vector3 viewAngleA = DirectionFromAngle(-fieldOfViewAngle / 2, false);
        Vector3 viewAngleB = DirectionFromAngle(fieldOfViewAngle / 2, false);

        Gizmos.DrawLine(transform.position, transform.position + viewAngleA * lookRadius);
        Gizmos.DrawLine(transform.position, transform.position + viewAngleB * lookRadius);

        Gizmos.color = Color.yellow;
        Gizmos.DrawRay(transform.position, (player.position - transform.position).normalized * lookRadius);

        // Draw a line from the enemy to the waypoint
        if (waypoints.Length > 0)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, waypoints[currentWaypointIndex].position);
        }

        // Draw lines to visualize the NavMeshAgent's path
        if (agent != null)
        {
            NavMeshPath path = new NavMeshPath();
            agent.CalculatePath(waypoints[currentWaypointIndex].position, path);
            Vector3 previousCorner = transform.position;
            foreach (Vector3 corner in path.corners)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawLine(previousCorner, corner);
                previousCorner = corner;
            }
        }
    }

    Vector3 DirectionFromAngle(float angleInDegrees, bool angleIsGlobal)
    {
        if (!angleIsGlobal)
        {
            angleInDegrees += transform.eulerAngles.y;
        }
        return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
    }

    void RestartPatrol()
    {
        StartCoroutine(Patrol());
    }

IEnumerator Patrol()
{
    while (true)
    {
        agent.SetDestination(GetRandomWaypoint().transform.position);
        Debug.Log("Setting destination to random waypoint");

        while (Vector3.Distance(transform.position, GetRandomWaypoint().position) > agent.stoppingDistance + 0.1f)
        {
            Debug.Log("Moving to waypoint, distance remaining: " + Vector3.Distance(transform.position, GetRandomWaypoint().position));
            // Debug.Log("Current velocity: " + agent.velocity);
            yield return null;
        }

        Debug.Log("Reached random waypoint");
        Debug.Log("Starting to look around");

        for (float rotated = 0; rotated < 360; rotated += 10)
        {
            if (!agent.isStopped) // If the enemy starts chasing the player, break the rotation
            {
                Debug.Log("Interrupted while looking around");
                break;
            }

            transform.rotation = Quaternion.Euler(0, rotated, 0);
            yield return new WaitForSeconds(0.2f); // Wait a little bit between each rotation
        }

        Debug.Log("Finished looking around");
        Debug.Log("Waiting at waypoint");

        // Add delay here if you want the enemy to wait at each waypoint
        yield return new WaitForSeconds(2f);
    }
}

Transform GetRandomWaypoint()
{
    int randomIndex = Random.Range(0, waypoints.Length);
    return waypoints[randomIndex].transform;
}
}


