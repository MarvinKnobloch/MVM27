using System;
using UnityEngine;

// This class existed prior, but there were no uses of it in the game. Marked obsolete as WallCralwer appears to be hte only enemy that is used.
[Obsolete]
public class PlatformCrawler : MonoBehaviour
{
	[Header("Enemy Settings")]
	[SerializeField] private float speed = 2f;
	[SerializeField] private bool moveRight = true;

	[Header("Collision Detection")]
	[SerializeField] private Transform wallCheck;
	[SerializeField] private LayerMask groundLayer;

	private Rigidbody2D rb;

	void Start()
	{
		rb = GetComponent<Rigidbody2D>();
	}

	void Update()
	{
		Move();

		// Turn when hit a wall
		if (HitsWall())
		{
			Flip();
		}
	}

	private void Move()
	{
		float moveDirection = moveRight ? 1f : -1f;
		rb.linearVelocity = new Vector2(moveDirection * speed, rb.linearVelocity.y);
	}

	private bool HitsWall()
	{
		// Wall Check
		return Physics2D.Raycast(wallCheck.position, moveRight ? Vector2.right : Vector2.left, 0.1f, groundLayer);
	}

	private void Flip()
	{
		moveRight = !moveRight;
		transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
	}
}
