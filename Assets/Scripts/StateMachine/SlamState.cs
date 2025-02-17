using UnityEngine;

public class SlamState : EnemyState
{
	public void EnterState(FlyingEnemy enemy)
    {
        Debug.Log("Entering Slam State");

        // turn off collider
        enemy.damageCollider.enabled = false;
        enemy.spriteObject.SetActive(false);

        // reset speed
        enemy.GetComponent<Rigidbody2D>().linearVelocity = Vector2.zero;

        // slam move
        enemy.GetComponent<Rigidbody2D>().linearVelocity = new Vector2(0, -enemy.slamSpeed);
    }

	public void UpdateState(FlyingEnemy enemy)
	{
		//stop at ground
		if (enemy.transform.position.y <= enemy.groundY - 0.43f) // Added offset to reach ground not sure whats wrong ***WILL FIX LATER***
		{
			// snap to adjusted ground level
			enemy.transform.position = new Vector3(enemy.transform.position.x, enemy.groundY - 0.42f, enemy.transform.position.z);

			// reset speed
			enemy.GetComponent<Rigidbody2D>().linearVelocity = Vector2.zero;

			Debug.Log("Enemy hit the ground! Staying for " + enemy.slamStayTime + " seconds.");

			// reset slam pause
			enemy.StartCoroutine(enemy.ReturnToOriginalHeight());
		}
	}

	public void ExitState(FlyingEnemy enemy)
	{
		Debug.Log("Exiting Slam State");
	}
}
