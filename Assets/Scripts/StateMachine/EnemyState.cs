public interface EnemyState
{
	void EnterState(FlyingEnemy enemy);
	void UpdateState(FlyingEnemy enemy);
	void ExitState(FlyingEnemy enemy);
}
