﻿using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Health))]
public class FlyerOld : MonoBehaviour
{
    private enum AttackTypes
    {
        /// <summary>
        /// a single shot periodically
        /// </summary>
        SingleShot,
        /// <summary>
        /// shoots multiple projectiles at once for less damage
        /// </summary>
        ScatterShot
    }

    private enum MovementType
    {
        /// <summary>
        /// Wanders a medium distance from their start location. Will attack on sight and chase a distance.
        /// </summary>
        Wander,
        /// <summary>
        /// Stays in one place, facing one direction. Low range of chase. Acts defensive, not offensive.
        /// </summary>
        Guard,
        /// <summary>
        /// Moves from point to point. Low range of chase. Smaller chase distance than wander, returns to patroling after attack.
        /// </summary>
        Patrol
    }

    private enum DetectionState
    {
        /// <summary>
        /// No target detected, do normal duties
        /// </summary>
        Idle,
        /// <summary>
        /// Thinks a target is near and is investigating
        /// </summary>
        Suspicous,
        /// <summary>
        /// Knows of target and is in combat mode
        /// </summary>
        Alert,
        /// <summary>
        /// Lost target and is returning to previous position (at faster speed)
        /// </summary>
        Lost,
        /// <summary>
        /// States the enemy is out of bounds and will return to their start position
        /// </summary>
        OutOfBounds
    }

    private enum CombatState
    {
        /// <summary>
        /// Enemy is not in combat
        /// </summary>
        None,
        /// <summary>
        /// Enemy is chasing target and wont attack until in range.
        /// This is when first sighting player or lost player
        /// </summary>
        Chasing,
        /// <summary>
        /// Enemy is currently attacking target
        /// </summary>
        Attacking,
        /// <summary>
        /// Enemy has attacked and and now might relocate
        /// </summary>
        PostAttackWait,
        /// <summary>
        /// Enemy is dead (playing out animations until destroy)
        /// </summary>
        Death,
        /// <summary>
        /// Enemy was hit, movement will be frozen
        /// </summary>
        Hit
    }

    private enum MovementFrequency
    {
        /// <summary>
        /// The enemy will move a little while not alert
        /// </summary>
        Little,
        /// <summary>
        /// The enemy will move some while not alert
        /// </summary>
        Normal,
        /// <summary>
        /// The enemy will move alot while not alert
        /// </summary>
        Alot
    }

    [Header("Components")]
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Animator animator;
    [SerializeField] private Health healthComponent;

    [Header("Height Control")]
    [SerializeField] private float minimumHeightFromGround = 2f;
    [SerializeField] private LayerMask groundLayerMask;

    [Header("Idle")]
    [SerializeField, Min(0f)] private float walkSpeed = 5.0f;
    [SerializeField] private MovementType movementType = MovementType.Wander;
    [Tooltip("How often this will move while on wander or patrol mode.")]
    [SerializeField] private MovementFrequency movementFrequency = MovementFrequency.Normal;
    [Tooltip("The radius distance the enemy can wander from its start point.")]
    [SerializeField, Min(0f)] private float wanderDistance = 2f;
    [SerializeField] private Transform[] patrolPoints;  // TODO: consider caching this is Vector2 on startup to avoid constant cast to Rigidbody position

    [Header("Detection")]
    [SerializeField, Min(0f)] private float visionDistance = 5.0f;
    [Tooltip("Select the layers vision can raycast against. Make sure things like enemies are unchecked.")]
    [SerializeField] private LayerMask visionLayerMask;

    [Header("Combat")]
    [SerializeField, Min(0f)] private float chaseSpeed = 3.0f;
    [Tooltip("this is the max area an aenemy is willing to move to while alert.")]
    [SerializeField, Min(0f)] private float chaseDistance = 14.0f;
    [Tooltip("The time in seconds we will look for the target after losing them")]
    [SerializeField, Min(0f)] private float timeToSearchWhenLost = 10f;
    [Tooltip("The desired distance to be above the target")]
    [SerializeField, Min(0f)] private float desiredAttackHeight = 2f;
    [Tooltip("The desired distance to be from the target")]
    [SerializeField, Min(0f)] private float desiredAttackRange = 3f;
    [SerializeField] private AttackTypes attackType = AttackTypes.SingleShot;
    [Tooltip("The time in seconds between shots")]
    [SerializeField, Min(0f)] private float attackRate = 3f;
    [Tooltip("The time in seconds to freeze when being hit")]
    [SerializeField, Min(0f)] private float hitFreezeTime = 1f;
    [Tooltip("The force to push this back when being hit")]
    [SerializeField, Min(0f)] private float hitPushbackForce = .4f;
    [Tooltip("The position to spawn the attack")]
    [SerializeField] private Transform attackSpawnPosition;
    [SerializeField] private FlyerAttack standardShotPrefab;
    [SerializeField] private FlyerAttack scatterShotPrefab;

    private Transform combatTarget;
    private Vector2 movementTarget; // this is the target to fly to while not alert
    private Vector2 startPosition;
    private bool targetInSight;
    private Vector2 targetLastKnownPosition;
    private float targetLastSeenTime;
    private DetectionState detectionState = DetectionState.Idle;
    private DetectionState lastDetectionState = DetectionState.Idle;
    private CombatState combatState = CombatState.None;
    private CombatState lastCombatState = CombatState.None;
    private float nextMovementTime;
    private int patrolIndex;
    private Vector2 desiredAttackPosition;
    private bool inAttackRange;
    private float nextAttackTime;
    private float attackCastTime; // the time we cast the attack
    private float hitTime; // the time we were hit

    private const float ATTACK_ANIM_BUFFER = 1f; // the time in seconds to wait from starting animation to cast
    private const float OUT_OF_BOUNDS_SPEED_BOOST = 1.5f;
    private const float DESIRED_ATTACK_RANGE_BUFFER = 0.7f; // we have a desired attack range, but the player will likely move while we prepare to attack. This buffer helps that situation.
    private const float DEATH_DESTROY_TIME = 1.5f;

    private const string FLAP_ANIM = "Flap";
    private const string HIT_ANIM = "Hit";
    private const string DIE_ANIM = "Die";

    private void Awake()
    {
        if (rb == null)
            rb = GetComponent<Rigidbody2D>();
        if (healthComponent == null)
            healthComponent = GetComponent<Health>();

        healthComponent.hitEvent.AddListener(OnHit);
        healthComponent.dieEvent.AddListener(OnDie);
    }

    private void Start()
    {
        combatTarget = Player.Instance.transform;
        startPosition = rb.position;
    }

    private void Update()
    {
        targetInSight = CheckVision(transform, combatTarget, visionDistance, false, visionLayerMask);

        if (targetInSight)
        {
            targetLastKnownPosition = (Vector2)combatTarget.position;
            targetLastSeenTime = Time.time;
        }

        // handle detection states
        lastDetectionState = detectionState;
        if (targetInSight && detectionState == DetectionState.Idle)
            detectionState = DetectionState.Alert;
        else if (!targetInSight && detectionState == DetectionState.Alert)
            detectionState = DetectionState.Lost;
        else if (targetInSight && detectionState == DetectionState.Lost)
            detectionState = DetectionState.Alert;
        else if (!targetInSight && detectionState == DetectionState.Lost && Time.time - targetLastSeenTime > timeToSearchWhenLost)
            detectionState = DetectionState.Idle;

        // handle combat states
        if (detectionState == DetectionState.Alert)
        {
            if (combatState == CombatState.None)
            {
                nextAttackTime = Time.time + attackRate;
                combatState = CombatState.Chasing;
            }
            else if (combatState == CombatState.Chasing && inAttackRange && Time.time > nextAttackTime)
            {
                combatState = CombatState.Attacking;
            }
            else if (combatState == CombatState.Attacking)
            {
                // because the attack will take time to cast, etc.. state changes must be handled there
                Attack();
            }
            else if (combatState == CombatState.PostAttackWait)
            {
                // leaving this blank, but in the future we could do something like "flee after every shot" or whatever
                combatState = CombatState.Chasing;
                attackCastTime = 0f;
            }
            else if (combatState == CombatState.Death)
            {

            }
            else if (combatState == CombatState.Hit)
            {
                if (Time.time - hitTime > hitFreezeTime)
                {
                    combatState = CombatState.Chasing;
                    hitTime = 0f;
                }
            }
        }
        else if (detectionState == DetectionState.Idle)
        {
            desiredAttackPosition = Vector2.zero;
            combatState = CombatState.None;
            nextAttackTime = 0f;
            attackCastTime = 0f;
            hitTime = 0f;
        }
        else
        {
            desiredAttackPosition = Vector2.zero;
            combatState = CombatState.Chasing;
            attackCastTime = 0f;
            hitTime = 0f;
        }
    }

    private void FixedUpdate()
    {
        if (detectionState == DetectionState.Idle)
        {
            targetLastKnownPosition = Vector2.zero;
            targetLastSeenTime = 0f;

            HandleNormalMovement();
        }
        else if (detectionState == DetectionState.Suspicous)
        {
            // TODO: Add support later
        }
        else if (detectionState == DetectionState.Alert)
        {
            nextMovementTime = 0f;
            movementTarget = Vector2.zero;

            HandleAlertMovement();
        }
        else if (detectionState == DetectionState.Lost)
        {
            HandleLostMovement();
        }
        else if (detectionState == DetectionState.OutOfBounds)
        {
            HandleOOBMovement();
        }
    }

    private void HandleNormalMovement()
    {
        if (movementType == MovementType.Wander)
        {
            if (movementTarget != Vector2.zero && rb.position.Approximately(movementTarget) == false)
            {
                Vector2 direction = (movementTarget - rb.position).normalized;
                rb.MovePosition(rb.position + direction * walkSpeed * Time.fixedDeltaTime);
            }
            else
            {
                if (nextMovementTime == 0f)
                {
                    // decide the time to move next
                    nextMovementTime = GetMovementFrequencyTime(movementFrequency);
                }
                else if (Time.time > nextMovementTime)
                {
                    // pick a new movement position
                    movementTarget = startPosition + (Random.insideUnitCircle * wanderDistance);
                    nextMovementTime = 0f;
                }
            }
        }
        else if (movementType == MovementType.Guard)
        {
            if (rb.position.Approximately(startPosition) == false)
            {
                Vector2 direction = (startPosition - rb.position).normalized;
                rb.MovePosition(rb.position + direction * walkSpeed * Time.fixedDeltaTime);
            }
        }
        else if (movementType == MovementType.Patrol)
        {
            if (movementTarget != Vector2.zero && rb.position.Approximately(movementTarget) == false)
            {
                Vector2 direction = (movementTarget - rb.position).normalized;
                rb.MovePosition(rb.position + direction * walkSpeed * Time.fixedDeltaTime);
            }
            else
            {
                if (nextMovementTime == 0f)
                {
                    // decide the time to move next
                    nextMovementTime = GetMovementFrequencyTime(movementFrequency);
                }
                else if (Time.time > nextMovementTime)
                {
                    patrolIndex++;
                    if (patrolIndex >= patrolPoints.Length)
                        patrolIndex = 0;

                    // pick a new movement position
                    movementTarget = (Vector2)patrolPoints[patrolIndex].position;
                    startPosition = movementTarget; // TODO: Decide if I like this. The OutOfBounds check is based on start position. Either I change start position or add special checks for patrol.
                    nextMovementTime = 0f;
                }
            }
        }
    }

    private void HandleAlertMovement()
    {
        if (Vector2.Distance(rb.position, startPosition) > chaseDistance)
        {
            // we have left our chase distance, stop chasing and return to start
            detectionState = DetectionState.OutOfBounds;
            return;
        }

        // freeze movement if we have been hit
        if (combatState == CombatState.Hit)
            return;

        Vector2 combatTargetPosition = (Vector2)combatTarget.position;
        Vector2 directionToTarget = (combatTargetPosition - rb.position).normalized;
        var distanceToTarget = Vector2.Distance(rb.position, combatTargetPosition);
        inAttackRange = (desiredAttackRange > distanceToTarget);

        if (combatState == CombatState.Chasing)
        {
            // try to move to the desired attacking position, with a buffer incase the player tries to move
            //if (distanceToTarget > desiredAttackRange * DESIRED_ATTACK_RANGE_BUFFER)
            if (distanceToTarget > desiredAttackRange)
            {
                // get the desired position + buffer. The offset ensures we can be on the left/right of the target
                float xOffset = (rb.position.x < combatTargetPosition.x) ? -desiredAttackRange : desiredAttackRange;
                desiredAttackPosition = combatTargetPosition + new Vector2(xOffset * DESIRED_ATTACK_RANGE_BUFFER, desiredAttackHeight * DESIRED_ATTACK_RANGE_BUFFER);

                Vector2 desiredDirection = (desiredAttackPosition - rb.position).normalized;
                Vector2 newPosition = rb.position + desiredDirection * chaseSpeed * Time.fixedDeltaTime;
                newPosition.y = ClampDistanceToGround(newPosition, groundLayerMask, minimumHeightFromGround, chaseSpeed);
                rb.MovePosition(newPosition);
            }
        }
    }

    private void HandleLostMovement()
    {
        // chase to the last known position and wait until state changes to idle
        if (rb.position.Approximately(targetLastKnownPosition) == false)
        {
            Vector2 direction = (targetLastKnownPosition - rb.position).normalized;
            Vector2 newPosition = rb.position + direction * chaseSpeed * Time.fixedDeltaTime;
            newPosition.y = ClampDistanceToGround(newPosition, groundLayerMask, minimumHeightFromGround, chaseSpeed);
            rb.MovePosition(newPosition);
        }
    }

    private void HandleOOBMovement()
    {
        // double time it back to start so the target cannot cheese the system by playing with enemy bounds
        if (rb.position.Approximately(startPosition) == false)
        {
            Vector2 direction = (startPosition - rb.position).normalized;
            rb.MovePosition(rb.position + direction * chaseSpeed * OUT_OF_BOUNDS_SPEED_BOOST * Time.fixedDeltaTime);
        }
        else
        {
            detectionState = DetectionState.Idle;
        }
    }

    private void Attack()
    {
        if (attackCastTime == 0f)
        {
            // TODO: play attack animation
            attackCastTime = Time.time;
        }
        else if (Time.time > attackCastTime + ATTACK_ANIM_BUFFER)
        {
            // now spawn the projectile
            var flyerAttack = Instantiate(standardShotPrefab, attackSpawnPosition);
            flyerAttack.Init(combatTarget);
            flyerAttack.Cast();
            nextAttackTime = Time.time + attackRate;
            combatState = CombatState.PostAttackWait;
        }
    }

    // this is triggered from the health component
    private void OnHit()
    {
        animator.Play(HIT_ANIM);
        combatState = CombatState.Hit;
        hitTime = Time.time;

        if (hitPushbackForce > 0f)
        {
            // hit does not provide the object that hit us, so we assume the player direction is the same
            var direction = (Player.Instance.rb.position - rb.position).normalized;
            var newPosition = rb.position - direction * hitPushbackForce;
            rb.MovePosition(newPosition);
        }
    }

    // this is triggered from the health component
    private void OnDie()
    {
        combatState = CombatState.Death;
        animator.Play(DIE_ANIM);
        Destroy(gameObject, DEATH_DESTROY_TIME);
    }

    private static float ClampDistanceToGround(Vector2 position, LayerMask layerMask, float minimumHeight, float speed)
    {
        // TODO: Consider using something other than infinity
        RaycastHit2D hit = Physics2D.Raycast(position, Vector2.down, Mathf.Infinity, layerMask);

        if (hit.collider != null)
        {
            float minimumPoint = hit.point.y + minimumHeight;
            if (position.y < minimumPoint)
            {
                return Mathf.MoveTowards(position.y, minimumPoint, speed * Time.fixedDeltaTime);
            }
        }

        return position.y;
    }

    /// <summary>
    /// Returns the time in seconds movement should occur based on the provided frequency
    /// </summary>
    /// <param name="movementFrequency"></param>
    /// <returns></returns>
    private static float GetMovementFrequencyTime(MovementFrequency movementFrequency)
    {
        float frequencyTime = movementFrequency switch
        {
            MovementFrequency.Little => 10f,
            MovementFrequency.Normal => 5f,
            MovementFrequency.Alot => 2f,
            _ => 5f
        };

        return Time.time + frequencyTime;
    }

    private static bool CheckVision(Transform self, Transform target, float visionDistance, bool forwardVisionOnly, LayerMask layerMask)
    {
        float distanceToTarget = Vector3.Distance(self.position, target.position);

        if (distanceToTarget <= visionDistance)
        {
            Vector3 directionToTarget = (target.position - self.position).normalized;

            if (forwardVisionOnly)
            {
                float dotProduct = Vector2.Dot(self.right.normalized, directionToTarget);
                if (dotProduct < 0)
                    return false;
            }

            RaycastHit2D hit = Physics2D.Raycast(self.position, directionToTarget, visionDistance, layerMask);

            if (hit && hit.transform == target)
                return true;
        }

        return false;
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        // wander area
        if (movementType == MovementType.Wander)
        {
            Gizmos.color = Color.green;
            if (Application.isPlaying)
                Gizmos.DrawWireSphere(startPosition, wanderDistance);
            else
                Gizmos.DrawWireSphere(transform.position, wanderDistance);
        }

        // idle walk target
        Gizmos.color = Color.green;
        if (movementTarget != Vector2.zero)
            Gizmos.DrawSphere(movementTarget, 0.1f);

        // chase area
        if (detectionState == DetectionState.Lost)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(startPosition, chaseDistance);
        }

        // vision area
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, visionDistance);

        // last known position of target
        if (targetLastKnownPosition != Vector2.zero)
            Gizmos.DrawSphere(targetLastKnownPosition, 0.1f);

        if (detectionState == DetectionState.Alert)
        {
            // attack range
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, desiredAttackRange);

            // desired attacking position
            if (targetLastKnownPosition != Vector2.zero)
                Gizmos.DrawSphere(desiredAttackPosition, 0.1f);
        }

        // detection state
        Vector3 textPosition = transform.position + (Vector3.up * 1f) + (Vector3.left * 0.5f);
        string message = $"{detectionState} :: {combatState}";
        UnityEditor.Handles.Label(textPosition, message);

        if (combatState != CombatState.None)
        {
            // time left to next attack
            float attackTimeDiff = Mathf.Round((nextAttackTime - Time.time) * 10f) / 10f;
            message = $"{Mathf.Clamp(attackTimeDiff, 0f, attackTimeDiff)}";
            UnityEditor.Handles.Label(textPosition + Vector3.up * .5f, message);
        }
    }
#endif
}