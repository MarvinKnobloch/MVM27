using UnityEngine;

public class JoesMovingPlatform : MonoBehaviour
{
	public Transform[] waypoints;
	public float speed;
	private int currentWaypointIndex = 0;
	private Vector3 lastPosition;

	void Start()
	{
		lastPosition = transform.position;
	}

	void Update()
	{
		if (waypoints.Length == 0) return;

		// Movement
		Vector3 targetPosition = waypoints[currentWaypointIndex].position;
		Vector3 direction = (targetPosition - transform.position).normalized;
		transform.position += direction * speed * Time.deltaTime;

		// Next Point
		if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
		{
			currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length;
		}
	}

	void FixedUpdate()
	{
		Vector3 platformMovement = transform.position - lastPosition;
		lastPosition = transform.position;

		foreach (Collider2D col in Physics2D.OverlapBoxAll(transform.position, GetComponent<BoxCollider2D>().bounds.size, 0))
		{
			if (col.CompareTag("Player"))
			{
				Rigidbody2D playerRb = col.GetComponent<Rigidbody2D>();
				if (playerRb != null)
				{
					playerRb.position += (Vector2)platformMovement; //Keeping player on platform needs more work
				}
			}
		}
	}
}
