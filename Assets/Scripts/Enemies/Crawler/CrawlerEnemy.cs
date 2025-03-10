using System;
using UnityEngine;

// TODO
// Right now this crawler can only go either horizontal or veritical
// It cannot be horizontal, an then crawl up a wall. If we decide we wan tthat,
// I will need to update the script to do so. I chose not to do because of the
// complexity to make such a transition look good.
//
// Also, I disabled vertical until we confirm we need it. Vertical requires ground detection
// so that we can sure the sprite is facing the right direction

public class CrawlerEnemy : MonoBehaviour
{
    private enum MovementType { Horizontal, Vertical }

    [Header("Components")]
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Animator animator;
    [SerializeField] private Health healthComponent;

    [Header("Config")]
    [SerializeField] private float speed = 2f;
    //[Tooltip("Crawlsers can only move either hoizontal or veritcal along the waypoints.")]
    //[SerializeField] private MovementType moveType = MovementType.Horizontal;
    private MovementType moveType = MovementType.Horizontal;
    [Tooltip("The waypoint to walk to and from.")]
    [SerializeField] private Transform waypoint;
    [Tooltip("The damage dealt to the player if they collide")]
    [SerializeField] private int damage = 1;

    private Vector2 startPosition = Vector2.zero;
    private Vector2 moveDirection = Vector2.zero;
    private bool movingTowardsWaypoint = true;
    
    private const float WAYPOINT_PROXIMITY = 0.1f;
    private const float DEATH_DESTROY_TIME = 0.5f;

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
    }

    private void FixedUpdate()
    {
        Vector2 targetPosition = (movingTowardsWaypoint) ? (Vector2)waypoint.position : startPosition;
        if (NearPosition(targetPosition))
        {
            movingTowardsWaypoint = !movingTowardsWaypoint;
            targetPosition = (movingTowardsWaypoint) ? waypoint.position : startPosition;
        }

        moveDirection = (targetPosition - rb.position).normalized;

        // Basically, whatever the startposition is for either x/y (based on the enum) we will stay that the whole way. No gravity
        if (moveType == MovementType.Horizontal)
            moveDirection.y = 0f;
        else
            moveDirection.x = 0f;

        rb.MovePosition(rb.position + (moveDirection * speed * Time.fixedDeltaTime));
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Player.Instance.health.TakeDamage(damage, false);
        }
    }

    private bool NearPosition(Vector2 targePosition)
    {
        if (moveType == MovementType.Horizontal)
            return (Mathf.Abs(rb.position.x - targePosition.x) < WAYPOINT_PROXIMITY);
        else
            return (Mathf.Abs(rb.position.y - targePosition.y) < WAYPOINT_PROXIMITY);
    }

    private void UpdateSpriteDirection(Vector3 direction)
    {
        if (moveType == MovementType.Horizontal)
        {
            if (direction.x > 0)
                transform.rotation = Quaternion.Euler(0, 0, 0);
            else
                transform.rotation = Quaternion.Euler(0, 180, 0);
        }
        else
        {
            if (direction.y > 0)
                transform.rotation = Quaternion.Euler(0, 0, -90);
            else
                transform.rotation = Quaternion.Euler(0, 0, 90);
        }
    }

    private void OnHit()
    {
        // todo play hit anim
    }

    private void OnDie()
    {
        // todo play death anim
        Destroy(gameObject, DEATH_DESTROY_TIME);
    }
}
