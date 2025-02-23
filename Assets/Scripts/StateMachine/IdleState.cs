using UnityEngine;

public class IdleState : EnemyState
{
	public override void EnterState(FlyingEnemy enemy)
	{
		Debug.Log("Enemy Entered Idle State"); 
	}
	public override void UpdateState(FlyingEnemy enemy)
	{
		enemy.HandleMovement(); 
	}

	public override void ExitState(FlyingEnemy enemy)
	{
		Debug.Log("Enemy Exiting Idle State");
	}
}
