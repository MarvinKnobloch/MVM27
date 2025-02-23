using UnityEngine;

public class PlayerJoe : MonoBehaviour
{
	public static PlayerJoe Instance { get; private set; }

	[Header("Movement Settings")]
	public float moveSpeed = 8f;
	public float jumpForce = 16f;
	public int extraJumps = 1;
	public float moveSmoothTime = 0.1f;

	[Header("Coyote Time Settings")]
	public float coyoteTime = 0.2f;
	private float coyoteTimeCounter;

	[Header("Gravity Modifiers")]
	public float fallMultiplier = 2.5f;
	public float lowJumpMultiplier = 2f;

	[Header("Ground Check Settings")]
	public Transform groundCheck;
	public float groundCheckRadius = 0.2f;
	public LayerMask groundLayer;

	[Header("Dash Settings")]
	public float dashSpeed = 20f;
	public float dashTime = 0.2f;
	public float dashCooldown = 1f;

	[Header("Shooting Settings")]
	public GameObject bulletPrefab;
	public Transform bulletSpawn;
	public float bulletSpeed = 800f;

	[Header("Rope Climbing Settings")]
	public float ropeClimbSpeed = 5f;
	private bool isOnRope = false;

	private int jumpCount;
	private bool isGrounded;
	private bool isDashing;
	private float dashTimeLeft;
	private float lastDashTime;
	private float velocityXSmoothing;

	private Rigidbody2D rb;
	private Animator animator;
	private SpriteRenderer spriteRenderer;
	private SwingingRope currentRope;

	private void Awake()
	{
		if (Instance != null && Instance != this)
		{
			Destroy(gameObject);
			return;
		}
		Instance = this;
	}

	void Start()
	{
		rb = GetComponent<Rigidbody2D>();
		rb.interpolation = RigidbodyInterpolation2D.Interpolate;
		animator = GetComponent<Animator>();
		spriteRenderer = GetComponent<SpriteRenderer>();
		jumpCount = extraJumps;
	}

	void Update()
	{
		isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
		if (isGrounded)
		{
			coyoteTimeCounter = coyoteTime;
			jumpCount = extraJumps;
		}
		else
		{
			coyoteTimeCounter -= Time.deltaTime;
		}

		if (isOnRope)
		{
			rb.linearVelocity = Vector2.zero;
			rb.gravityScale = 0;

			float verticalInput = Input.GetAxisRaw("Vertical");
			rb.linearVelocity = new Vector2(0, verticalInput * ropeClimbSpeed);

			float moveInput = Input.GetAxisRaw("Horizontal");
			if (moveInput != 0 && currentRope != null)
			{
				currentRope.ApplySwingForce(moveInput);
			}

			if (Input.GetButtonDown("Jump"))
			{
				isOnRope = false;
				rb.gravityScale = 1;
				rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
				transform.parent = null;
				currentRope = null;
			}

			return;
		}

		if (isDashing)
		{
			rb.linearVelocity = new Vector2(transform.localScale.x * dashSpeed, 0);
			dashTimeLeft -= Time.deltaTime;
			if (dashTimeLeft <= 0)
			{
				isDashing = false;
				rb.linearVelocity = Vector2.zero;
			}
			return;
		}

		float moveInputX = Input.GetAxisRaw("Horizontal");
		float targetVelocityX = moveInputX * moveSpeed;
		float newVelocityX = Mathf.SmoothDamp(rb.linearVelocity.x, targetVelocityX, ref velocityXSmoothing, moveSmoothTime);
		rb.linearVelocity = new Vector2(newVelocityX, rb.linearVelocity.y);

		if (moveInputX != 0)
			transform.localScale = new Vector3(Mathf.Sign(moveInputX), 1, 1);

		if (Input.GetButtonDown("Jump") && (coyoteTimeCounter > 0 || jumpCount > 0))
		{
			rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
			if (coyoteTimeCounter <= 0) jumpCount--;
			coyoteTimeCounter = 0;
		}

		if (rb.linearVelocity.y != 0)
		{
			float multiplier = rb.linearVelocity.y > 0 && !Input.GetButton("Jump") ? lowJumpMultiplier : fallMultiplier;
			rb.linearVelocity += Vector2.up * Physics2D.gravity.y * (multiplier - 1) * Time.deltaTime;
		}

		if (Input.GetButtonDown("Fire2") && Time.time >= lastDashTime + dashCooldown)
		{
			isDashing = true;
			dashTimeLeft = dashTime;
			lastDashTime = Time.time;
		}

		if (Input.GetButtonDown("Fire1"))
		{
			Shoot();
		}

		if (animator != null)
		{
			animator.SetFloat("Speed", Mathf.Abs(moveInputX));
			animator.SetBool("isGrounded", isGrounded);
		}
	}

	void Shoot()
	{
		if (bulletPrefab == null || bulletSpawn == null)
		{
			Debug.LogWarning("Bullet Prefab or Bullet Spawn is not assigned!");
			return;
		}

		GameObject bullet = Instantiate(bulletPrefab, bulletSpawn.position, Quaternion.identity);
		Rigidbody2D bulletRb = bullet.GetComponent<Rigidbody2D>();
		if (bulletRb != null)
		{
			Vector2 shootDirection = transform.right * Mathf.Sign(transform.localScale.x);
			bulletRb.linearVelocity = shootDirection * bulletSpeed;
		}
	}

	private void OnTriggerEnter2D(Collider2D other)
	{
		if (other.CompareTag("Rope"))
		{
			isOnRope = true;
			rb.linearVelocity = Vector2.zero;

			transform.parent = other.transform;

			currentRope = other.GetComponent<SwingingRope>();
		transform.position = new Vector3(other.transform.position.x, transform.position.y, transform.position.z);
		}
	}

	private void OnTriggerExit2D(Collider2D other)
	{
		if (other.CompareTag("Rope"))
		{
			isOnRope = false;
			rb.gravityScale = 1;
			transform.parent = null;
			currentRope = null;
		}
	}
}
