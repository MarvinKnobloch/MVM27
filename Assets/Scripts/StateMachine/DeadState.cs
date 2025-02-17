using UnityEngine;

public class DeadState : EnemyState
{
	public void EnterState(FlyingEnemy enemy)
	{
		//for debug
		Debug.Log("Enemy is Dead");
		enemy.gameObject.SetActive(false);
	}

	public void UpdateState(FlyingEnemy enemy) { }

	public void ExitState(FlyingEnemy enemy) { }
}
