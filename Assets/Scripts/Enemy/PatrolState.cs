using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatrolState : EnemyBaseState
{
    public override void EnterState(EnemyController enemy)
    {
        Debug.Log("Entered Patrol State");
        // set patrol animation
        enemy.animator.SetBool("Walking", true);
        enemy.animator.SetBool("Chasing", false);
        enemy.animator.SetBool("Idle", false);
        enemy.agent.speed = enemy.speed;
    }

    public override void Update(EnemyController enemy)
    {
        // Patrol between waypoints
        enemy.Patrol();

        // check for player in field of view and proximity
        if (enemy.IsPlayerInFieldOfView() && enemy.IsPlayerInProximity())
        {
            enemy.TransitionToState(enemy.chaseState);
        }
    }
}
