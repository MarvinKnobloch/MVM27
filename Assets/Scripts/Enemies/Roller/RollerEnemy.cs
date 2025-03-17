using System;
using UnityEngine;

public class RollerEnemy : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Animator animator;
    [SerializeField] private Health healthComponent;
    [SerializeField] private RollerDamageCollider damageCollider;

    [Header("Config")]
    [SerializeField, Min(0f)] private float speed = 2f;
    [Tooltip("The speed we move if the target is in sight.")]
    [SerializeField, Min(0f)] private float chaseSpeed = 5f;
    [Tooltip("The waypoint to walk to and from.")]
    [SerializeField] private Transform waypoint;
    [Tooltip("The damage dealt to the target if they collide")]
    [SerializeField, Min(0)] private int damage = 1;
    [Tooltip("The time to freeze the enemy on hit. Set to 0 to disable.")]
    [SerializeField, Min(0f)] private float freezeOnHitTime = .3f;
    [SerializeField, Min(0f)] private float visionRange = 8f;
    [SerializeField] private LayerMask visionMask;
    [Tooltip("The time in seconds we are to wait before allowing a hit on the target again.")]
    [SerializeField, Min(0f)] private float attackBuffer = 1f;
    [SerializeField, Min(0f)] private float maxDistanceFromPatrol = 4f;
    [SerializeField] private LayerMask groundCheckMask;
    [Tooltip("When the roller chases the player, it will go this far past the player as its target. Careful its not greater than MaxDistanceFromPatrol")]
    [SerializeField] private float overRollDistance = 2f;

    private Transform target;
    private Vector2 startPosition = Vector2.zero;
    private Vector2 moveDirection = Vector2.zero;
    private bool movingTowardsWaypoint = true;
    private float hitTime = 0f;
    private bool dead = false;
    private bool targetInSight = false;
    private Vector2 lastKnownTargetPosition = Vector2.zero;
    private float lastTimeHitPlayer = 0f; // this the time we hit the player, not the player attacking us

    private const float WAYPOINT_PROXIMITY = 0.1f;
    private const float DEATH_DESTROY_TIME = 0.5f;
    private const float VISION_ANGLE = 45f;
    private const float GROUND_CHECK = 0.5f;

    private const string IDLE_ANIM = "Idle";
    private const string HIT_ANIM = "Hit";
    private const string DIE_ANIM = "Die";

    private void Awake()
    {
        if (rb == null)
            throw new ArgumentNullException(nameof(rb));
        if (healthComponent == null)
            throw new System.ArgumentNullException(nameof(healthComponent));
        if (animator == null)
            throw new System.ArgumentNullException(nameof(animator));
        if (waypoint == null)
            throw new ArgumentNullException(nameof(waypoint));
        if (damageCollider == null)
            throw new ArgumentNullException(nameof(damageCollider));

        healthComponent.hitEvent.AddListener(OnHit);
        healthComponent.dieEvent.AddListener(OnDie);
        damageCollider.OnTriggerEnter += TrigerEnter;
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
            targetInSight = CheckVision() && IsGroundAhead();
            if (targetInSight)
                lastKnownTargetPosition = (Vector2)target.position;
        }

        if (hitTime != 0f && Time.time - hitTime > freezeOnHitTime)
            hitTime = 0f;

        if (lastTimeHitPlayer > 0f && Time.time - lastTimeHitPlayer > attackBuffer)
            lastTimeHitPlayer = 0f;
    }

    private void FixedUpdate()
    {
        if (hitTime != 0f || dead)
            return;

        if (lastKnownTargetPosition != Vector2.zero)
        {
            // make sure cap movement from our patrol point
            if (DistanceOutsidePatrolPoints() > maxDistanceFromPatrol || !IsGroundAhead())
            {
                lastKnownTargetPosition = Vector2.zero;
            }
            else
            {
                // calculate the position to roll at (past the player)
                Vector2 targetPosition = lastKnownTargetPosition + (moveDirection * overRollDistance);
                if (NearPosition(targetPosition))
                {
                    // we hit our target position, turn around and check for the player
                    moveDirection.x *= -1;
                    targetInSight = CheckVision();
                    lastKnownTargetPosition = (targetInSight) ? (Vector2)target.position : Vector2.zero;
                }
                else
                    rb.linearVelocity = new Vector2(moveDirection.x * chaseSpeed, rb.linearVelocity.y);
            }
        }
        else
        {
            // move to waypoint
            Vector2 targetPosition = (movingTowardsWaypoint) ? (Vector2)waypoint.position : startPosition;
            if (NearPosition(targetPosition))
            {
                movingTowardsWaypoint = !movingTowardsWaypoint;
                targetPosition = (movingTowardsWaypoint) ? waypoint.position : startPosition;
            }

            moveDirection = (targetPosition - rb.position).normalized;
            moveDirection.y = 0f;

            rb.linearVelocity = new Vector2(moveDirection.x * speed, rb.linearVelocity.y);
        }
    }

    // this function is attached to the event from roller damage collider
    public void TrigerEnter(Collider2D collider)
    {
        if (dead || lastTimeHitPlayer > 0f)
            return;

        if (collider.gameObject.CompareTag(target.tag))
        {
            Player.Instance.health.PlayerTakeDamage(damage, false, true);
            lastTimeHitPlayer = Time.time;
        }
    }

    private bool CheckVision()
    {
        bool sawTarget = false;
        Vector2 directionToTarget = ((Vector2)target.position - rb.position).normalized;
        if (Mathf.Sign(directionToTarget.x) == Mathf.Sign(moveDirection.x))
        {
            float distanceToTarget = Vector2.Distance(rb.position, (Vector2)target.position);
            if (distanceToTarget <= visionRange)
            {
                float angleToTarget = Vector2.Angle(moveDirection, directionToTarget);
                if (angleToTarget < VISION_ANGLE * 0.5f)
                {
                    var visionHit = Physics2D.Raycast(rb.position, directionToTarget, visionRange, visionMask);
                    sawTarget = visionHit.collider != null && visionHit.collider.CompareTag(target.tag);
                }
            }
        }
        return sawTarget;
    }

    private float DistanceOutsidePatrolPoints()
    {
        // figure out which waypoint is left/right
        var leftWaypoint = (startPosition.x < waypoint.position.x) ? startPosition.x : waypoint.position.x;
        var rightWaypoint = (startPosition.x > waypoint.position.x) ? startPosition.x : waypoint.position.x;

        // if we are in between the patrol points, just return 0
        if (rb.position.x >= leftWaypoint && rb.position.x <= rightWaypoint)
            return 0f;

        // if the target is between the patrol pionts, return 0
        if (target.position.x >= leftWaypoint && target.position.x <= rightWaypoint)
            return 0f;

        // if target is closer to waypoints then us, return 0f
        float centerPoint = (leftWaypoint + rightWaypoint) / 2f;
        float distanceFromCenter = Mathf.Abs(rb.position.x - centerPoint);
        float targetDistanceFromCenter = Mathf.Abs(target.position.x - centerPoint);
        if (distanceFromCenter > targetDistanceFromCenter)
            return 0f;

        var targetPosition = (moveDirection.x > 0) ? rightWaypoint : leftWaypoint;
        return Mathf.Abs(rb.position.x - targetPosition);
    }

    private bool NearPosition(Vector2 targetPosition)
    {
        return (Mathf.Abs(rb.position.x - targetPosition.x) < WAYPOINT_PROXIMITY);
    }

    private bool IsGroundAhead()
    {
        RaycastHit2D hit = Physics2D.Raycast(rb.position + moveDirection * GROUND_CHECK, Vector2.down, GROUND_CHECK, groundCheckMask);
        return hit.collider != null;
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

        // ground check raycast
        Gizmos.color = Color.green;
        Vector2 rayStart = rb.position + moveDirection * GROUND_CHECK;
        Vector2 rayEnd = rayStart + (Vector2.down * GROUND_CHECK);
        Gizmos.DrawLine(rayStart, rayEnd);

        if (lastKnownTargetPosition != Vector2.zero)
        {
            // Gizmos.color = Color.red;
            // Gizmos.DrawSphere(lastKnownTargetPosition, 0.1f);

            Gizmos.color = Color.yellow;
            Gizmos.DrawSphere(lastKnownTargetPosition + moveDirection * overRollDistance, 0.1f);
        }

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

        //UnityEditor.Handles.Label(rb.position + Vector2.up * 0.5f, targetInSight ? "Target in Sight" : "No Target");
        //UnityEditor.Handles.Label(rb.position + Vector2.up * 0.5f, IsGroundAhead() ? "Ground Ahead" : "No Ground Ahead");
    }
#endif
}
