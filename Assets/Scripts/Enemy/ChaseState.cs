using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChaseState : EnemyBaseState
{
    private float timePlayerOutOfSight;

    public override void EnterState(EnemyController enemy)
    {
        Debug.Log("Entered Chase State");
        // set chase animation
        enemy.animator.SetBool("Walking", true);
        enemy.animator.SetBool("Idle", false);
        enemy.agent.isStopped = false;
        timePlayerOutOfSight = 0f;
    }

    public override void Update(EnemyController enemy)
    {
        if (enemy.IsPlayerAttackable())
        {
            Debug.Log("Player is attackable, transitioning to AttackState.");
            enemy.TransitionToState(enemy.attackState);
        }
        else if (!enemy.IsPlayerInFieldOfView() || !enemy.IsPlayerInProximity())
        {
            timePlayerOutOfSight += Time.deltaTime;
            if (timePlayerOutOfSight >= 5f)
                enemy.TransitionToState(enemy.patrolState);
        }
        else
        {
            enemy.Chase();
            timePlayerOutOfSight = 0f;
        }
    }
}
