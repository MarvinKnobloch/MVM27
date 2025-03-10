using System;
using UnityEngine;

public class ThwomperEnemy : MonoBehaviour
{
    private enum MovementState { Idle, Falling, Grounded, Rising }

    [Header("Components")]
    [SerializeField] private Collider2D col;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Animator animator;
    [SerializeField] private Health healthComponent;

    [Header("Config")]
    [SerializeField, Min(0f)] private float speed = 2f;
    [Tooltip("The speed we move if the target is in sight.")]
    [SerializeField, Min(0f)] private float fallSpeed = 10f;
    [SerializeField, Min(0f)] private float riseSpeed = 4f;
    [Tooltip("The waypoint to walk to and from.")]
    [SerializeField] private Transform waypoint;
    [Tooltip("The damage dealt to the target if they collide")]
    [SerializeField, Min(0)] private int damage = 1;
    [Tooltip("The time to freeze the enemy on hit. Set to 0 to disable.")]
    [SerializeField, Min(0f)] private float freezeOnHitTime = .3f;
    [SerializeField] private LayerMask visionMask;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundMask;
    [Tooltip("The time in seconds we are to wait before rising.")]
    [SerializeField, Min(0f)] private float timeToWaitOnGround = 3f;

    private Transform target;
    private Vector2 startPosition = Vector2.zero;
    private Vector2 moveDirection = Vector2.zero;
    private bool movingTowardsWaypoint = true;
    private float hitTime = 0f;
    private bool dead = false;
    private bool targetInSight = false;
    private MovementState movementState = MovementState.Idle;
    private Vector2 positionBeforeFall = Vector2.zero;
    private float groundedTime = 0f;


    private const float WAYPOINT_PROXIMITY = 0.1f;
    private const float DEATH_DESTROY_TIME = 1.5f;
    private const float GROUND_CHECK_RADIUS = 0.1f;
    private const float VISION_RANGE = 20f;

    private const string IDLE_ANIM = "Idle";
    private const string HIT_ANIM = "Hit";
    private const string DIE_ANIM = "Die";
    private const string FALL_ANIM = "Fall";

    private void Awake()
    {
        if (col == null)
            throw new ArgumentNullException(nameof(col));
        if (rb == null)
            throw new ArgumentNullException(nameof(rb));
        if (healthComponent == null)
            throw new System.ArgumentNullException(nameof(healthComponent));
        if (animator == null)
            throw new System.ArgumentNullException(nameof(animator));
        if (waypoint == null)
            throw new ArgumentNullException(nameof(waypoint));
        if (groundCheck == null)
            throw new ArgumentNullException(nameof(groundCheck));

        healthComponent.hitEvent.AddListener(OnHit);
        healthComponent.dieEvent.AddListener(OnDie);

        // Helps keep the waypoint at a sensible place
        if (transform.position.y != waypoint.position.y)
            waypoint.position = new Vector3(waypoint.position.x, transform.position.y, waypoint.position.z);
    }

    private void Start()
    {
        startPosition = rb.position;
        target = Player.Instance.transform;
        healthComponent.AllowDamage = false; // we only allow damage while Grounded
    }

    private void Update()
    {
        if (movementState == MovementState.Idle)
        {
            var visionHit = Physics2D.Raycast(rb.position, Vector2.down, VISION_RANGE, visionMask);
            targetInSight = visionHit.collider != null && visionHit.collider.CompareTag(target.tag);
        }

        if (targetInSight && movementState == MovementState.Idle)
        {
            movementState = MovementState.Falling;
            positionBeforeFall = rb.position;
            animator.Play(FALL_ANIM);
        }
        else if (movementState == MovementState.Grounded && Time.time - groundedTime > timeToWaitOnGround)
        {
            movementState = MovementState.Rising;
            healthComponent.AllowDamage = false;
        }
        else if (movementState == MovementState.Grounded)
        {
            healthComponent.AllowDamage = true;
        }
        else if (movementState == MovementState.Rising && rb.position.y >= positionBeforeFall.y)
        {
            movementState = MovementState.Idle;
            positionBeforeFall = Vector2.zero;
            animator.Play(IDLE_ANIM);
            groundedTime = 0f;
            col.excludeLayers = 0;
        }

        if (hitTime != 0f && Time.time - hitTime > freezeOnHitTime)
            hitTime = 0f;
    }

    private void FixedUpdate()
    {
        if (hitTime != 0f || dead)
            return;

        if (movementState == MovementState.Idle)
        {
            Vector2 targetPosition = (movingTowardsWaypoint) ? (Vector2)waypoint.position : startPosition;
            if (NearPosition(targetPosition))
            {
                movingTowardsWaypoint = !movingTowardsWaypoint;
                targetPosition = (movingTowardsWaypoint) ? waypoint.position : startPosition;
            }

            moveDirection = (targetPosition - rb.position).normalized;
            moveDirection.y = 0f;

            rb.linearVelocity = new Vector2(moveDirection.x * speed, 0f);
        }
        else if (movementState == MovementState.Falling)
        {
            rb.linearVelocity = new Vector2(0f, -fallSpeed);

            if (Physics2D.OverlapCircle(groundCheck.position, GROUND_CHECK_RADIUS, groundMask))
            {
                movementState = MovementState.Grounded;
                groundedTime = Time.time;
            }
        }
        else if (movementState == MovementState.Grounded)
        {
            rb.linearVelocityY = 0f;
        }
        else if (movementState == MovementState.Rising)
        {
            rb.linearVelocity = new Vector2(0f, riseSpeed);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (dead || movementState != MovementState.Falling)
            return;

        if (collision.gameObject.CompareTag(target.tag))
        {
            Debug.Log("hit target");
            Player.Instance.health.TakeDamage(damage, false);
            col.excludeLayers |= 1 << target.gameObject.layer;
        }
    }

    private bool NearPosition(Vector2 targetPosition)
    {
        return (Mathf.Abs(rb.position.x - targetPosition.x) < WAYPOINT_PROXIMITY);
    }

    // this is triggered from the health component
    private void OnHit()
    {
        hitTime = Time.time;
        animator.Play(HIT_ANIM);
    }

    // this is triggered from the health component
    private void OnDie()
    {
        dead = true;
        animator.Play(DIE_ANIM);
        Destroy(gameObject, DEATH_DESTROY_TIME);
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(rb.position, rb.position + Vector2.down * VISION_RANGE);

        if (waypoint != null)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawSphere(waypoint.position, 0.1f);
        }

        if (startPosition != Vector2.zero)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawSphere(startPosition, 0.1f);
        }

        if (groundCheck != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(groundCheck.position, GROUND_CHECK_RADIUS);
        }

        UnityEditor.Handles.Label(rb.position + Vector2.up * 1.75f, movementState.ToString());
    }

    private void OnValidate()
    {
        // Helps keep the waypoint at a sensible place
        if (waypoint != null && transform.position.y != waypoint.position.y)
        {
            waypoint.position = new Vector3(waypoint.position.x, transform.position.y, waypoint.position.z);
            UnityEditor.EditorUtility.SetDirty(waypoint);
        }
    }
#endif
}
