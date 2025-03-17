using System;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

// TODO
// Refactor this so the crawler basically crawls on the ground,
// at any direction or angle // to its target.

public class CrawlerEnemy : MonoBehaviour
{
    private enum MovementType { Horizontal, Vertical }

    [Header("Components")]
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Animator animator;
    [SerializeField] private Health healthComponent;
    [SerializeField] private CrawlerDamageCollider damageCollider;

    [Header("Config")]
    [SerializeField] private float speed = 1.2f;
    [SerializeField] private LayerMask groundLayerMask;
    [Tooltip("The waypoint to walk to and from.")]
    [SerializeField] private Transform waypoint;
    [Tooltip("The damage dealt to the player if they collide")]
    [SerializeField] private int damage = 1;
    [SerializeField, Min(0f)]
    private float freezeOnHitTime = .3f;
    [Tooltip("The time in seconds we are to wait before allowing a hit on the target again.")]
    [SerializeField, Min(0f)] private float attackBuffer = 1f;
    [Tooltip("How much to knock the enemy back when hit.")]
    [SerializeField, Min(0f)] private float knockbackForce = 2.5f;

    private Transform target;
    private Vector2 startPosition = Vector2.zero;
    private Vector2 moveDirection = Vector2.zero;
    private bool movingTowardsWaypoint = true;
    private float hitTime = 0f;
    private bool dead = false;
    private float lastTimeHitPlayer = 0f; // this the time we hit the player, not the player attacking us

    private const float WAYPOINT_PROXIMITY = 0.1f;
    private const float DEATH_DESTROY_TIME = 0.5f;

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

        if (hitTime != 0f && Time.time - hitTime > freezeOnHitTime)
            hitTime = 0f;

        if (lastTimeHitPlayer > 0f && Time.time - lastTimeHitPlayer > attackBuffer)
            lastTimeHitPlayer = 0f;
    }

    private void FixedUpdate()
    {
        if (hitTime != 0f || dead)
            return;

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

    public void TrigerEnter(Collider2D collider)
    {
        Debug.Log(collider.gameObject.name, collider.gameObject);
        if (dead || lastTimeHitPlayer > 0f)
            return;

        if (collider.gameObject.CompareTag(target.tag))
        {
            Player.Instance.health.PlayerTakeDamage(damage, false, true);
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

        var knockbackDirection = (rb.position - (Vector2)target.transform.position).normalized;
        rb.linearVelocity = Vector2.zero;
        rb.AddForce(knockbackDirection * knockbackForce, ForceMode2D.Impulse);
    }

    // this is triggered from the health component
    private void OnDie()
    {
        dead = true;
        animator.Play(DIE_ANIM);
        Destroy(gameObject, DEATH_DESTROY_TIME);
    }

    private void OnDrawGizmos()
    {
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
    }
}
