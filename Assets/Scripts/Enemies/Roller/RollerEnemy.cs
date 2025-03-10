using System;
using UnityEngine;

public class RollerEnemy : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private Collider2D col;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Animator animator;
    [SerializeField] private Health healthComponent;

    [Header("Config")]
    [SerializeField, Min(0f)] private float speed = 2f;
    [Tooltip("The speed we move if the target is in sight.")]
    [SerializeField, Min(0f)] private float chaseSpeed = 5f;
    [Tooltip("The waypoint to walk to and from.")]
    [SerializeField] private Transform waypoint;
    [Tooltip("The damage dealt to the player if they collide")]
    [SerializeField, Min(0)] private int damage = 1;
    [Tooltip("The time to freeze the enemy on hit. Set to 0 to disable.")]
    [SerializeField, Min(0f)] private float freezeOnHitTime = .3f;
    [SerializeField, Min(0f)] private float visionRange = 8f;
    [SerializeField] private LayerMask visionMask;
    [Tooltip("The time in seconds we are to wait before allowing a hit on the target again.")]
    [SerializeField, Min(0f)] private float attackBuffer = 1f;

    private Transform target;
    private Vector2 startPosition = Vector2.zero;
    private Vector2 moveDirection = Vector2.zero;
    private bool movingTowardsWaypoint = true;
    private float hitTime = 0f;
    private bool dead = false;
    private bool targetInSight = false;
    private float lastTimeHitPlayer = 0f; // this the time we hit the player, not the player attacking us

    private const float WAYPOINT_PROXIMITY = 0.1f;
    private const float DEATH_DESTROY_TIME = 0.5f;

    private const string IDLE_ANIM = "Idle";
    private const string HIT_ANIM = "Hit";
    private const string DIE_ANIM = "Die";

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

        healthComponent.hitEvent.AddListener(OnHit);
        healthComponent.dieEvent.AddListener(OnDie);
    }

    private void Start()
    {
        startPosition = rb.position;
        target = Player.Instance.transform;
    }

    private void Update()
    {
        if (moveDirection != Vector2.zero)
            UpdateSpriteDirection(moveDirection);
        
        if (moveDirection != Vector2.zero)
        {
            var visionHit = Physics2D.Raycast(rb.position, moveDirection, visionRange, visionMask);
            targetInSight = visionHit.collider != null && visionHit.collider.CompareTag("Player");
        }

        if (hitTime != 0f && Time.time - hitTime > freezeOnHitTime)
            hitTime = 0f;

        if (lastTimeHitPlayer > 0f && Time.time - lastTimeHitPlayer > attackBuffer)
        {
            lastTimeHitPlayer = 0f;
            col.excludeLayers = 0;
        }
    }

    private void FixedUpdate()
    {
        if (hitTime != 0f || dead)
            return;

        Vector2 targetPosition = (movingTowardsWaypoint) ? (Vector2)waypoint.position : startPosition;
        if (NearPosition(targetPosition))
        {
            movingTowardsWaypoint = !movingTowardsWaypoint;
            targetPosition = (movingTowardsWaypoint) ? waypoint.position : startPosition;
        }

        moveDirection = (targetPosition - rb.position).normalized;
        moveDirection.y = 0f;

        float movementSpeed = (targetInSight || lastTimeHitPlayer != 0f) ? chaseSpeed : speed;

        rb.linearVelocity = new Vector2(moveDirection.x * movementSpeed, rb.linearVelocity.y);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (dead || lastTimeHitPlayer > 0f)
            return;
        
        if (collision.gameObject.CompareTag(target.tag))
        {
            Player.Instance.health.TakeDamage(damage, false);
            lastTimeHitPlayer = Time.time;
            col.excludeLayers |= 1 << target.gameObject.layer;
        }
    }

    private bool NearPosition(Vector2 targetPosition)
    {
        return (Mathf.Abs(rb.position.x - targetPosition.x) < WAYPOINT_PROXIMITY);
    }

    private void UpdateSpriteDirection(Vector3 direction)
    {
        if (direction.x > 0)
            transform.rotation = Quaternion.Euler(0, 0, 0);
        else
            transform.rotation = Quaternion.Euler(0, 180, 0);
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
        Vector2 direction = moveDirection != Vector2.zero ? moveDirection : Vector2.right;
        Gizmos.DrawLine(rb.position, rb.position + direction * visionRange);

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

        UnityEditor.Handles.Label(rb.position + Vector2.up * 0.5f, targetInSight ? "Target in Sight" : "No Target");
    }
#endif
}
