using UnityEngine;

public class WallCrawlerEnemy : MonoBehaviour
{
	public Transform[] waypoints;
	public float speed;

	private int currentWaypointIndex = 0;

	void Update()
	{
		Vector3 targetPosition = waypoints[currentWaypointIndex].position;
		Vector3 direction = (targetPosition - transform.position).normalized;
		transform.position += direction * speed * Time.deltaTime;

		// Update direction
		UpdateSpriteDirection(direction);

		// Move to the nxt point
		if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
		{
			currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length;
		}
	}

	private void UpdateSpriteDirection(Vector3 direction)
	{
		// Turn
		if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
		{
			if (direction.x > 0)
				transform.rotation = Quaternion.Euler(0, 0, 180);
			else
				transform.rotation = Quaternion.Euler(0, 0, 0);
		}
		else
		{
			if (direction.y > 0)
				transform.rotation = Quaternion.Euler(0, 0, -90);
			else
				transform.rotation = Quaternion.Euler(0, 0, 90);
		}
	}
}
