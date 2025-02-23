using UnityEngine;

public class DeadState : EnemyState
{
    public override void EnterState(FlyingEnemy enemy)
    {
        Debug.Log("Enemy is now dead.");
        enemy.gameObject.SetActive(false);
    }

    public override void UpdateState(FlyingEnemy enemy)
    {
        //Add if needed
    }

    public override void ExitState(FlyingEnemy enemy)
    {
        Debug.Log("Enemy respawning...");
        enemy.gameObject.SetActive(true);
    }
}
