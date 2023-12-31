using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleState : EnemyBaseState
{
    private float idleWaitTime;

    public override void EnterState(EnemyController enemy)
    {
        // set idle animation
        Debug.Log("Entered Idle State");
        enemy.animator.SetBool("Idle", true);
        enemy.animator.SetBool("Walking", false);
        enemy.animator.SetBool("Chasing", false);
        idleWaitTime = Random.Range(2, 5); // wait for 2 to 4 seconds

    }

    public override void Update(EnemyController enemy)
    {
        if (idleWaitTime <= 0)
        {
            do
            {
                enemy.currentWaypointIndex = Random.Range(0, enemy.waypoints.Length);
            }
            while (Vector3.Distance(enemy.transform.position, enemy.waypoints[enemy.currentWaypointIndex].position) < 0.1f);

            enemy.TransitionToState(enemy.patrolState);
        }
        else
        {
            idleWaitTime -= Time.deltaTime;
        }

        // check for player in field of view and proximity
        if (enemy.IsPlayerInFieldOfView() && enemy.IsPlayerInProximity())
        {
            enemy.TransitionToState(enemy.chaseState);
        }
        if (enemy.isPetrified)
        {
            enemy.TransitionToState(enemy.petrifiedState);
        }
    }
}