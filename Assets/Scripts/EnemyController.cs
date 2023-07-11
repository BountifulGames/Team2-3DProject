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
    public Animator monsterAnimator;

    public float attackRange = 2f;  // Attack range
    public float attackDelay = 1f;  // Attack delay
    private float lastAttackTime = 0;  // When the last attack happened
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

        if (monsterAnimator.GetCurrentAnimatorStateInfo(0).IsName("Attack") &&
            monsterAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f)
        {
            monsterAnimator.SetBool("isAttacking", false);
            monsterAnimator.SetBool("Walking", true);
            agent.isStopped = false;  // Resume moving
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
        return !Physics.Raycast(transform.position, direction, distance, LayerMask.GetMask("Wall"));
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
        StopCoroutine(Patrol());

        // Only start pursuing the player if the enemy is not currently attacking
        if (!monsterAnimator.GetBool("isAttacking"))
        {
            agent.isStopped = false;
            agent.destination = player.position;
            monsterAnimator.SetBool("Walking", true);
        }

        // If the player is close enough to the enemy, start attacking
        if (Vector3.Distance(transform.position, player.position) <= attackRange)
        {
            agent.isStopped = true;  // Stop moving when attacking
            monsterAnimator.SetBool("isAttacking", true);
            monsterAnimator.SetBool("Walking", false);
        }
    }

    void AttackPlayer()
    {
        monsterAnimator.SetBool("isAttacking", true);
        lastAttackTime = Time.time;
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
            monsterAnimator.SetBool("Walking", true);
            // Get a random waypoint once at the start of the patrol cycle
            Transform waypoint = GetRandomWaypoint();
            agent.SetDestination(waypoint.position);
            Debug.Log("Setting destination to waypoint at position: " + waypoint.position);

            while (Vector3.Distance(transform.position, waypoint.position) > agent.stoppingDistance + 0.1f)
            {
                Debug.Log("Moving to waypoint, distance remaining: " + Vector3.Distance(transform.position, waypoint.position));
                Debug.Log("Waypoint position is still: " + waypoint.position);
                yield return null;
            }

            Debug.Log("Reached waypoint at position: " + waypoint.position);
            Debug.Log("Waiting at waypoint");
            monsterAnimator.SetBool("Idle", true);
            // Add delay here if you want the enemy to wait at each waypoint
            yield return new WaitForSeconds(3f);
        }
    }

    Transform GetRandomWaypoint()
    {
        int randomIndex = Random.Range(0, waypoints.Length);
        return waypoints[randomIndex].transform;
    }
}



