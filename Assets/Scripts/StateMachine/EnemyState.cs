using UnityEngine;

public abstract class EnemyState
{
    public abstract void EnterState(FlyingEnemy enemy);
    public abstract void UpdateState(FlyingEnemy enemy);
    public abstract void ExitState(FlyingEnemy enemy);
}
