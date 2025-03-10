using System;
using UnityEngine;

// TODO: Either implement gravity, or switch to not kinmatic

public class RollerEnemy : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Animator animator;
    [SerializeField] private Health healthComponent;

    [Header("Config")]
    [SerializeField, Min(0f)] private float speed = 2f;
    [Tooltip("The waypoint to walk to and from.")]
    [SerializeField] private Transform waypoint;
    [Tooltip("The damage dealt to the player if they collide")]
    [SerializeField, Min(0)] private int damage = 1;
    [Tooltip("The time to freeze the enemy on hit. Set to 0 to disable.")]
    [SerializeField, Min(0f)] private float freezeOnHitTime = .3f;

    private Vector2 startPosition = Vector2.zero;
    private Vector2 moveDirection = Vector2.zero;
    private bool movingTowardsWaypoint = true;
    private float hitTime = 0f;
    private bool dead = false;

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
    }

    private void Start()
    {
        startPosition = rb.position;
    }

    private void Update()
    {
        if (moveDirection != Vector2.zero)
            UpdateSpriteDirection(moveDirection);

        if (hitTime != 0f && Time.time - hitTime > freezeOnHitTime)
            hitTime = 0f;
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
        moveDirection.y = 0f; // dont allow vertical movement

        rb.MovePosition(rb.position + (moveDirection * speed * Time.fixedDeltaTime));
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (dead)
            return;

        if (collision.gameObject.CompareTag("Player"))
            Player.Instance.health.TakeDamage(damage, false);
    }

    private bool NearPosition(Vector2 targePosition)
    {
        return (Mathf.Abs(rb.position.x - targePosition.x) < WAYPOINT_PROXIMITY);
    }

    private void UpdateSpriteDirection(Vector3 direction)
    {
        if (direction.x > 0)
            transform.rotation = Quaternion.Euler(0, 0, 0);
        else
            transform.rotation = Quaternion.Euler(0, 180, 0);
    }

    private void OnHit()
    {
        hitTime = Time.time;
        animator.Play(HIT_ANIM);
    }

    private void OnDie()
    {
        dead = true;
        animator.Play(DIE_ANIM);
        Destroy(gameObject, DEATH_DESTROY_TIME);
    }
}
