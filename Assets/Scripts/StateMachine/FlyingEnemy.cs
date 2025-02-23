using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FlyingEnemy : MonoBehaviour
{
	public float speed = 3f;
	public float slamSpeed = 10f;
	public int health = 3;
	public GameObject player;

	[SerializeField] private bool useWaypoints = false;
	[SerializeField] private Transform[] waypoints;
	private int currentWaypointIndex = 0;
	
	[SerializeField] private float patrolDistance = 5f;
	private Vector3 startPos;
	private int moveDirection = 1;

	[SerializeField] public Collider2D damageCollider;
	[SerializeField] public GameObject spriteObject;
	[SerializeField] public float onTime = 10f;
	[SerializeField] public float offTime = 3f;

	[SerializeField] public float groundY = -2f;
	[SerializeField] public float slamStayTime = 2f;
	[SerializeField] public float returnSpeed = 3f;

	[SerializeField] private List<BulletDamageData> bulletDamageList = new List<BulletDamageData>();

	private EnemyState currentState;
	public IdleState idleState = new IdleState();
	public SlamState slamState = new SlamState();
	public DeadState deadState = new DeadState();

	private Rigidbody2D rb;
	private float originalHeight;
	private Coroutine toggleCoroutine;

	void Start()
	{
		rb = GetComponent<Rigidbody2D>();
		
		if (damageCollider == null || spriteObject == null)
		{
			Debug.LogError("DamageCollider or SpriteObject is not assigned in Inspector!");
			return;
		}

		damageCollider.enabled = false;
		spriteObject.SetActive(false);
		originalHeight = transform.position.y;
		startPos = transform.position;

		ChangeState(idleState);
		toggleCoroutine = StartCoroutine(ToggleColliderAndSprite());
	}

	void Update()
	{
		currentState?.UpdateState(this);
	}

	public void HandleMovement()
	{
		if (useWaypoints)
		{
			MoveBetweenWaypoints();
		}
		else
		{
			MoveBackAndForth();
		}
	}

	private void MoveBackAndForth()
	{
		transform.position += Vector3.right * moveDirection * speed * Time.deltaTime;

		if (Mathf.Abs(transform.position.x - startPos.x) >= patrolDistance)
		{
			moveDirection *= -1;
		}
	}

	private void MoveBetweenWaypoints()
	{
		if (waypoints == null || waypoints.Length < 2)
		{
			Debug.LogWarning("Please set enough waypoints!");
			return;
		}

		Transform targetWaypoint = waypoints[currentWaypointIndex];
		Vector3 direction = (targetWaypoint.position - transform.position).normalized;
		transform.position += direction * speed * Time.deltaTime;

		if (Vector3.Distance(transform.position, targetWaypoint.position) < 0.1f)
		{
			currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length;
		}
	}

	public void ChangeState(EnemyState newState)
	{
		if (currentState != null)
			currentState.ExitState(this);

		currentState = newState;
		currentState.EnterState(this);
	}

	private IEnumerator ToggleColliderAndSprite()
	{
		while (true)
		{
			damageCollider.enabled = true;
			spriteObject.SetActive(true);
			yield return new WaitForSeconds(onTime);

			damageCollider.enabled = false;
			spriteObject.SetActive(false);
			yield return new WaitForSeconds(offTime);
		}
	}

	public void TakeDamage(int damage)
	{
		health -= damage;
		Debug.Log("Enemy took " + damage + " damage! Health: " + health);

		if (health <= 0)
			ChangeState(deadState);
	}

	public void PrepareForSlam()
	{
		originalHeight = transform.position.y;
		damageCollider.enabled = false;

		if (toggleCoroutine != null)
		{
			StopCoroutine(toggleCoroutine);
			toggleCoroutine = null;
		}
	}

	public IEnumerator ReturnToOriginalHeight()
	{
		yield return new WaitForSeconds(slamStayTime);

		rb.linearVelocity = Vector2.zero;

		while (transform.position.y < originalHeight)
		{
			rb.linearVelocity = new Vector2(0, returnSpeed);
			yield return null;
		}

		transform.position = new Vector3(transform.position.x, originalHeight, transform.position.z);
		rb.linearVelocity = Vector2.zero;
		damageCollider.enabled = true;
		spriteObject.SetActive(true);

		if (toggleCoroutine == null)
		{
			toggleCoroutine = StartCoroutine(ToggleColliderAndSprite());
		}

		ChangeState(idleState);
	}

	void OnTriggerEnter2D(Collider2D other)
	{
		if (damageCollider.enabled && other.gameObject == player)
		{
			PrepareForSlam();
			ChangeState(slamState);
		}
	}

	public void OnHitByBullet(GameObject bullet)
	{
		foreach (BulletDamageData bulletData in bulletDamageList)
		{
			if (bulletData.bulletPrefab != null && bullet.CompareTag(bulletData.bulletPrefab.tag))
			{
				TakeDamage(bulletData.damageAmount);
				Destroy(bullet);
				return;
			}
		}

		Debug.Log("Bullet did not effect this enemy.");
	}
}
