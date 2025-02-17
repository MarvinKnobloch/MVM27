using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FlyingEnemy : MonoBehaviour
{
	public float speed = 3f;
	public float slamSpeed = 10f;
	public int health = 3;

	public GameObject player;

	[Header("Collider and Visibility Settings")]
	[SerializeField] public Collider2D damageCollider;
	[SerializeField] public GameObject spriteObject;
	[SerializeField] public float onTime = 10f;
	[SerializeField] public float offTime = 3f;

	[Header("Slam Attack Settings")]
	[SerializeField] public float groundY = -2f;
	[SerializeField] public float slamStayTime = 2f;
	[SerializeField] public float returnSpeed = 3f;

	[Header("Bullet Damage Settings")]
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

		ChangeState(idleState);
		toggleCoroutine = StartCoroutine(ToggleColliderAndSprite());
	}

	void Update()
	{
		currentState?.UpdateState(this);
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
			Debug.Log("Collider & Sprite ON");
			damageCollider.enabled = true;
			spriteObject.SetActive(true);
			yield return new WaitForSeconds(onTime);

			Debug.Log("Collider & Sprite OFF");
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
        // turn off collider when slamming
		damageCollider.enabled = false;
		// stop collider sequence
		if (toggleCoroutine != null)
		{
			StopCoroutine(toggleCoroutine);
			toggleCoroutine = null;
		}

		
	}

    public IEnumerator ReturnToOriginalHeight()
    {
        yield return new WaitForSeconds(slamStayTime); //rest on ground

        Debug.Log("Returning to original height");

        // return speed
        rb.linearVelocity = Vector2.zero;

        while (transform.position.y < originalHeight)
        {
            rb.linearVelocity = new Vector2(0, returnSpeed);
            yield return null;
        }

        // check final position
        transform.position = new Vector3(transform.position.x, originalHeight, transform.position.z);

        // stop and turn on collider
        rb.linearVelocity = Vector2.zero;
        damageCollider.enabled = true;
        spriteObject.SetActive(true);

        // toggle restert
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
		// Check if this bullet is listed incase enemy is emun to a bullet type.
		foreach (BulletDamageData bulletData in bulletDamageList)
		{
			if (bulletData.bulletPrefab != null && bullet.CompareTag(bulletData.bulletPrefab.tag))
			{
				TakeDamage(bulletData.damageAmount);
				Destroy(bullet); // Destroy bullet
				return;
			}
		}

		// ignore unknown emun bullets
		Debug.Log("Bullet did not affect this enemy.");
	}
}
