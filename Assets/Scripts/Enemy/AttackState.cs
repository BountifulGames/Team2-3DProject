using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackState : EnemyBaseState
{
    public override void EnterState(EnemyController enemy)
    {
        enemy.attackCoroutine = enemy.StartCoroutine(enemy.Attack());
        Debug.Log("Entered Attack State");   
        enemy.agent.isStopped = true; // Stop moving
        enemy.Attack(); // Call the attack function
        enemy.animator.SetBool("Walking", false);
        enemy.animator.SetBool("Idle", false);
        enemy.animator.SetBool("Chasing", false);
    }

    public override void Update(EnemyController enemy)
    {
        // If the player gets too far, go back to chase state
        if (!enemy.IsPlayerAttackable())
        {
            enemy.TransitionToState(enemy.chaseState);
        }
    }
    public override void ExitState(EnemyController enemy)
    {
        if (enemy.attackCoroutine != null)
        {
            enemy.StopCoroutine(enemy.attackCoroutine);
            enemy.attackCoroutine = null;
        }
    }
}
