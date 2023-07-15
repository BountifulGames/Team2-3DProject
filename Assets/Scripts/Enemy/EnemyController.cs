using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    public Transform player;
    public PlayerHealth playerHealth;
    public Transform[] waypoints;
    public float detectionRange = 10.0f;
    public float attackRange = 2.0f;
    public float fieldOfView = 60f;
    public float speed = 3f;
    public int currentWaypointIndex = 0;
    public float rotationSpeed;

    public EnemyBaseState currentState;
    public IdleState idleState;
    public PatrolState patrolState;
    public ChaseState chaseState;
    public AttackState attackState;

    public Animator animator;

    public NavMeshAgent agent; // The NavMeshAgent

    public Coroutine attackCoroutine;


    private void Start()
    {
        agent = GetComponent<NavMeshAgent>(); // Get the NavMeshAgent component
        agent.speed = speed; // Set the agent's speed
        idleState = new IdleState();
        patrolState = new PatrolState();
        chaseState = new ChaseState();
        attackState = new AttackState();

        currentState = patrolState;
    }

    private void Update()
    {
        currentState.Update(this);
    }

    public void TransitionToState(EnemyBaseState state)
    {
        currentState.ExitState(this);
        currentState = state;
        currentState.EnterState(this);
    }

    public void Patrol()
    {
        // If we've reached the waypoint, move on to the next one
        if (Vector3.Distance(transform.position, waypoints[currentWaypointIndex].position) < 1f)
        {
            TransitionToState(idleState);
        }
        else
        {
            Vector3 direction = (waypoints[currentWaypointIndex].position - transform.position).normalized;
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * rotationSpeed);

            // Set the agent's destination to the current waypoint
            agent.SetDestination(waypoints[currentWaypointIndex].position);
        }
    }

    public void Chase()
    {
        Vector3 direction = (player.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * rotationSpeed);

        // Set the agent's destination to the player
        agent.SetDestination(player.position);
    }

    public IEnumerator Attack()
    {
        animator.SetTrigger("Attack");

        while (IsPlayerAttackable())
        {
            animator.SetTrigger("Attack");
            yield return new WaitForSeconds(2f);
        }

        // If the player has gotten too far away, go back to chasing them
        TransitionToState(chaseState);
        
    }

    public bool IsPlayerInProximity()
    {
        // Check if the player is within detection range
        return Vector3.Distance(transform.position, player.position) < detectionRange;
    }

    public bool IsPlayerInFieldOfView()
    {
        // Calculate direction to player
        Vector3 dirToPlayer = (player.position - transform.position).normalized;

        // Calculate angle between forward direction and direction to player
        float angle = Vector3.Angle(transform.forward, dirToPlayer);

        // Player is in field of view if angle is less than or equal to half of field of view
        if (angle <= fieldOfView / 2f)
        {
            // Make sure there is no obstruction between the enemy and the player
            RaycastHit hit;
            if (Physics.Raycast(transform.position, dirToPlayer, out hit, detectionRange))
            {
                if (hit.transform == player)
                {
                    return true;
                }
            }
        }

        return false;
    }

    public bool IsPlayerAttackable()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        bool isAttackable = distanceToPlayer < attackRange;

        // Debug.Log("Player is attackable: " + isAttackable);

        return isAttackable;
    }

    public void DealDamageToPlayer()
    {
        PlayerHealth playerHealth = player.GetComponent<PlayerHealth>(); // Get the PlayerHealth component
        if (playerHealth != null)
        {
            playerHealth.TakeDamage(1); // Deal damage to the player
            playerHealth.StartBleeding(); // Start the bleed effect

        }
    }
}



