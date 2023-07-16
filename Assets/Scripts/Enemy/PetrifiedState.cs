using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PetrifiedState : EnemyBaseState
{
    public override void EnterState(EnemyController enemy)
    {
        Debug.Log("Entered Petrified State");
    }
    public override void Update(EnemyController enemy)
    {
        return;
    }
}
