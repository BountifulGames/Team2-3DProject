using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EnemyBaseState
{
    // These methods are virtual, meaning they can be overridden in child classes
    public virtual void EnterState(EnemyController enemy) { }
    public virtual void Update(EnemyController enemy) { }
    public virtual void ExitState(EnemyController enemy) { }

}
