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
        enemy.animator.SetBool("Idle", false);

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
