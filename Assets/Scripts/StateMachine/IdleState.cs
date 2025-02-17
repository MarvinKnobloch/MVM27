using UnityEngine;

public class IdleState : EnemyState
{
	private float direction = 1f;
	private float startY;
	private float swayFrequency = 3f;
	private float swayAmplitude = 0.1f;

	public void EnterState(FlyingEnemy enemy)
	{
		Debug.Log("Entering Idle State");
		startY = enemy.transform.position.y;
	}

	public void UpdateState(FlyingEnemy enemy)
	{
		// Move left and right
		float xPosition = enemy.transform.position.x + (direction * enemy.speed * Time.deltaTime);

		// sway
		float yPosition = startY + Mathf.Sin(Time.time * swayFrequency) * swayAmplitude;

		// update location
		enemy.transform.position = new Vector3(xPosition, yPosition, enemy.transform.position.z);

		// Change direction when limit hit
		if (xPosition > 4f) direction = -1f;
		if (xPosition < -4f) direction = 1f;

		// Check for player
		if (enemy.player != null && Vector3.Distance(enemy.transform.position, enemy.player.transform.position) < 5f)
		{
			Debug.Log("Player nearby! Be ready.");
		}
	}

	public void ExitState(FlyingEnemy enemy)
	{
		Debug.Log("Exiting Idle State");
	}
}
