using UnityEngine;
//I know Mervvel is codeing the player I just coded this to test out my enemy State Machine cause there was no code.
public class Player : MonoBehaviour
{
    // Singleton instance connection
    public static Player Instance { get; private set; }

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
    // Choose bullet
    public GameObject bulletPrefab;
    // Bullet spawn
    public Transform bulletSpawn;
    // bullet speed
    public float bulletSpeed = 800f;

    private int jumpCount;
    private bool isGrounded;
    private bool isDashing;
    private float dashTimeLeft;
    private float lastDashTime;
    private float velocityXSmoothing;

    private Rigidbody2D rb;
    private Animator animator;
    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        // Set up the singleton info.
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
        // smoother movement
        rb.interpolation = RigidbodyInterpolation2D.Interpolate;

        animator = GetComponent<Animator>();        // when animation is available
        spriteRenderer = GetComponent<SpriteRenderer>();  // if using a sprite renderer
        jumpCount = extraJumps;
    }

    void Update()
    {
        // Ground Check and Coyote Time Check as mentioned in metting
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

        // Dashing
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

        // Left and right movement
        float moveInput = Input.GetAxisRaw("Horizontal");
        float targetVelocityX = moveInput * moveSpeed;
        float newVelocityX = Mathf.SmoothDamp(rb.linearVelocity.x, targetVelocityX, ref velocityXSmoothing, moveSmoothTime);
        rb.linearVelocity = new Vector2(newVelocityX, rb.linearVelocity.y);

        // Change sprite facing direction
        if (moveInput != 0)
            transform.localScale = new Vector3(Mathf.Sign(moveInput), 1, 1);

        // Jump
        if (Input.GetButtonDown("Jump") && (coyoteTimeCounter > 0 || jumpCount > 0))
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            if (coyoteTimeCounter <= 0) jumpCount--;
            coyoteTimeCounter = 0;
        }

        // Gravity control
        if (rb.linearVelocity.y != 0)
        {
            float multiplier = rb.linearVelocity.y > 0 && !Input.GetButton("Jump") ? lowJumpMultiplier : fallMultiplier;
            rb.linearVelocity += Vector2.up * Physics2D.gravity.y * (multiplier - 1) * Time.deltaTime;
        }

        // Dash
        if (Input.GetButtonDown("Fire2") && Time.time >= lastDashTime + dashCooldown)
        {
            isDashing = true;
            dashTimeLeft = dashTime;
            lastDashTime = Time.time;
        }

        // Shooting
        if (Input.GetButtonDown("Fire1"))
        {
            Shoot();
        }

        // Animator updates when ready
        if (animator != null)
        {
            animator.SetFloat("Speed", Mathf.Abs(moveInput));
            animator.SetBool("isGrounded", isGrounded);
        }
    }

    // create bullet and assign speed
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
            // bullet direction
            Vector2 shootDirection = transform.right * Mathf.Sign(transform.localScale.x);
            bulletRb.linearVelocity = shootDirection * bulletSpeed;
        }
    }

    // Ground check
    private void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }

    // Collision detections
    private void OnCollisionEnter2D(Collision2D collision)
    {
#if UNITY_EDITOR
        Debug.Log("Player collided with " + collision.gameObject.name);
#endif
    }

    void OnTriggerEnter2D(Collider2D other)
    {
#if UNITY_EDITOR
        Debug.Log("Player entered with " + other.gameObject.name);
#endif
    }

    void OnTriggerExit2D(Collider2D other)
    {
#if UNITY_EDITOR
        Debug.Log("Player left with " + other.gameObject.name);
#endif
    }
}
