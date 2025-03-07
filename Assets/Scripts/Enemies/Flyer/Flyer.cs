using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Health))]
public class Flyer : MonoBehaviour
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
        /// Knows of target and is in combat mode
        /// </summary>
        Alert
    }

    private enum CombatState
    {
        /// <summary>
        /// Enemy is chasing target and wont attack until in range.
        /// This is when first sighting player or lost player
        /// </summary>
        PreAttack,
        /// <summary>
        /// Enemy is currently attacking target
        /// </summary>
        Attacking,
        /// <summary>
        /// Enemy has attacked and and now might relocate
        /// </summary>
        PostAttack,
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

    private struct CombatData
    {
        public CombatState State;

        /// <summary>
        /// not used, but good information if we want to add gameplay for it later
        /// </summary>
        public Vector2 TargetLastKnownPosition;

        /// <summary>
        /// the time in seconds we last saw the target
        /// </summary>
        public float TargetLastSeenTime;

        /// <summary>
        /// states if the target is in attack range or not
        /// </summary>
        public bool TargetInAttackRange;

        /// <summary>
        /// the time we can attack next (compare against Time.time)
        /// </summary>
        public float NextAttackTime;

        /// <summary>
        /// the time we cast the attack (compare against Time.time)
        /// </summary>
        public float AttackCastTime;

        /// <summary>
        /// the time we were hit (compare against Time.time)
        /// </summary>
        public float HitTime;

        /// <summary>
        /// the position we were in when we became alert (for repositining logic)
        /// </summary>
        public Vector2 BecameAlertPosition;
    }

    [Header("Components")]
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Animator animator;
    [SerializeField] private Health healthComponent;

    [Header("Idle")]
    [SerializeField, Min(0f)] private float flySpeed = 5.0f;
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
    [SerializeField, Min(0f)] private float combatFlySpeed = 5.0f;
    [Tooltip("The time in seconds until the flyer is no longer alert if it cannot find the target")]
    [SerializeField, Min(0f)] private float timeToLeaveCombat = 10f;
    [Tooltip("The time in seconds to freeze when being hit. Set to 0 to disable.")]
    [SerializeField, Min(0f)] private float hitFreezeTime = 1f;
    [Tooltip("The force to push this back when being hit. Set to 0 to disable.")]
    [SerializeField, Min(0f)] private float hitPushbackForce = .4f;

    [Header("Combat - Attack")]
    [SerializeField] private AttackTypes attackType = AttackTypes.SingleShot;
    [Tooltip("The desired distance to be from the target")]
    [SerializeField, Min(0f)] private float attackRange = 3f;
    [Tooltip("The time in seconds between shots")]
    [SerializeField, Min(0f)] private float attackRate = 3f;
    [Tooltip("The position to spawn the attack")]
    [SerializeField] private Transform attackSpawnPosition;
    [SerializeField] private FlyerAttack standardShotPrefab;
    [SerializeField] private FlyerAttack scatterShotPrefab;

    [Header("Combat - Reposition")]
    [Tooltip("The radius distance the enemy can reposition after attacking")]
    [SerializeField, Min(0f)] private float repositionDistance = 2f;
    [Tooltip("How often this will move while on reposition after attacking.")]
    [SerializeField] private MovementFrequency repositionFrequency = MovementFrequency.Alot;


    private Vector2 startPosition;
    private Transform combatTarget;
    private DetectionState detectionState = DetectionState.Idle;
    private int patrolIndex;
    private Vector2 movementTarget;
    private float nextMovementTime;
    private bool targetInSight;
    private CombatData combatData;

    /// <summary>
    /// the time in seconds to wait from starting animation to cast
    /// </summary>
    private const float ATTACK_ANIM_BUFFER = 1f;

    /// <summary>
    /// The time in seconds to delay the destory (for animations and such)
    /// </summary>
    private const float DEATH_DESTROY_TIME = 1.5f;

    private const string FLAP_ANIM = "Flap";
    private const string HIT_ANIM = "Hit";
    private const string DIE_ANIM = "Die";

    private void Awake()
    {
        if (rb == null)
            throw new System.ArgumentNullException(nameof(rb));
        if (healthComponent == null)
            throw new System.ArgumentNullException(nameof(healthComponent));
        if (animator == null)
            throw new System.ArgumentNullException(nameof(animator));

        healthComponent.hitEvent.AddListener(OnHit);
        healthComponent.dieEvent.AddListener(OnDie);
    }

    private void Start()
    {
        combatTarget = Player.Instance.transform;
        startPosition = rb.position;
        combatData = new CombatData();
    }

    private void Update()
    {
        // handle vision
        targetInSight = CheckVision(transform, combatTarget, visionDistance, false, visionLayerMask);
        if (targetInSight)
        {
            combatData.TargetLastKnownPosition = (Vector2)combatTarget.position;
            combatData.TargetLastSeenTime = Time.time;
        }

        // handle detection states
        if (targetInSight && detectionState == DetectionState.Idle)
        {
            nextMovementTime = 0f;
            movementTarget = Vector2.zero;
            combatData.BecameAlertPosition = rb.position;
            combatData.NextAttackTime = Time.time + attackRate;
            detectionState = DetectionState.Alert;
        }
        else if (!targetInSight && detectionState == DetectionState.Alert && Time.time - combatData.TargetLastSeenTime > timeToLeaveCombat)
        {
            nextMovementTime = 0f;
            movementTarget = Vector2.zero;
            combatData = new CombatData();
            detectionState = DetectionState.Idle;
        }

        // handle combat states
        if (detectionState == DetectionState.Alert)
        {
            if (combatData.State == CombatState.PreAttack && combatData.TargetInAttackRange && Time.time > combatData.NextAttackTime)
            {
                combatData.State = CombatState.Attacking;
            }
            else if (combatData.State == CombatState.Attacking)
            {
                // because the attack will take time to cast, etc.. state changes must be handled in Attack()
                Attack();
            }
            else if (combatData.State == CombatState.PostAttack)
            {
                // leaving this blank, but in the future we could do something like "flee after every shot" or whatever
                combatData.State = CombatState.PreAttack;
            }
            else if (combatData.State == CombatState.Death)
            {
                // do nothing
            }
            else if (combatData.State == CombatState.Hit)
            {
                if (Time.time - combatData.HitTime > hitFreezeTime)
                {
                    combatData.State = CombatState.PreAttack;
                    combatData.HitTime = 0f;
                }
            }
        }
    }

    private void FixedUpdate()
    {
        if (detectionState == DetectionState.Idle)
            HandleNormalMovement();
        else if (detectionState == DetectionState.Alert)
            HandleAlertMovement();
    }

    private void HandleNormalMovement()
    {
        if (movementType == MovementType.Wander)
        {
            if (movementTarget != Vector2.zero && rb.position.Approximately(movementTarget) == false)
            {
                Vector2 direction = (movementTarget - rb.position).normalized;
                rb.MovePosition(rb.position + direction * flySpeed * Time.fixedDeltaTime);
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
                rb.MovePosition(rb.position + direction * flySpeed * Time.fixedDeltaTime);
            }
        }
        else if (movementType == MovementType.Patrol)
        {
            if (movementTarget != Vector2.zero && rb.position.Approximately(movementTarget) == false)
            {
                Vector2 direction = (movementTarget - rb.position).normalized;
                rb.MovePosition(rb.position + direction * flySpeed * Time.fixedDeltaTime);
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
                    nextMovementTime = 0f;
                }
            }
        }
    }

    private void HandleAlertMovement()
    {
        // freeze movement if we have been hit
        if (hitFreezeTime > 0f && combatData.State == CombatState.Hit)
            return;

        Vector2 combatTargetPosition = (Vector2)combatTarget.position;
        Vector2 directionToTarget = (combatTargetPosition - rb.position).normalized;
        var distanceToTarget = Vector2.Distance(rb.position, combatTargetPosition);
        combatData.TargetInAttackRange = (attackRange > distanceToTarget);

        // while we are in pre attack phase, reposition
        if (combatData.State == CombatState.PreAttack)
        {
            if (movementTarget != Vector2.zero && rb.position.Approximately(movementTarget) == false)
            {
                Vector2 direction = (movementTarget - rb.position).normalized;
                rb.MovePosition(rb.position + direction * combatFlySpeed * Time.fixedDeltaTime);
            }
            else
            {
                if (nextMovementTime == 0f)
                {
                    // decide the time to move next
                    nextMovementTime = GetRepositionFrequencyTime(repositionFrequency);
                }
                else if (Time.time > nextMovementTime)
                {
                    // pick a new movement position
                    var fightingPosition = (movementType == MovementType.Patrol) ? combatData.BecameAlertPosition : startPosition;
                    movementTarget = fightingPosition + (Random.insideUnitCircle * repositionDistance);
                    nextMovementTime = 0f;
                }
            }
        }
    }

    private void Attack()
    {
        // NOTE: This function is expected to be called on Update()

        if (combatData.AttackCastTime == 0f)
        {
            // TODO: play cast attack animation
            combatData.AttackCastTime = Time.time;
        }
        else if (Time.time > combatData.AttackCastTime + ATTACK_ANIM_BUFFER)
        {
            // now spawn the projectile
            var flyerAttack = Instantiate(standardShotPrefab, attackSpawnPosition);
            flyerAttack.Init(combatTarget);
            flyerAttack.Cast();
            combatData.NextAttackTime = Time.time + attackRate;
            combatData.State = CombatState.PostAttack;
            combatData.AttackCastTime = 0f;
        }
    }

    // this is triggered from the health component
    private void OnHit()
    {
        animator.Play(HIT_ANIM);
        combatData.State = CombatState.Hit;
        combatData.HitTime = Time.time;

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
        combatData.State = CombatState.Death;
        animator.Play(DIE_ANIM);
        Destroy(gameObject, DEATH_DESTROY_TIME);
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

    /// <summary>
    /// Returns the time in seconds movement should occur based on the provided frequency
    /// </summary>
    /// <param name="movementFrequency"></param>
    /// <returns></returns>
    private static float GetRepositionFrequencyTime(MovementFrequency movementFrequency)
    {
        float frequencyTime = movementFrequency switch
        {
            MovementFrequency.Little => 5f,
            MovementFrequency.Normal => 3f,
            MovementFrequency.Alot => 1.5f,
            _ => 3f
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
        if (detectionState == DetectionState.Idle)
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
        }

        // vision area
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, visionDistance);

        if (detectionState == DetectionState.Alert)
        {
            // attack range
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, attackRange);

            // attack range
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(combatData.BecameAlertPosition, repositionDistance);

            // time left to next attack
            Vector3 tempTextPosition = transform.position + (Vector3.up * 1f) + (Vector3.left * 0.5f);
            float attackTimeDiff = Mathf.Round((combatData.NextAttackTime - Time.time) * 10f) / 10f;
            UnityEditor.Handles.Label(tempTextPosition + Vector3.up * .5f, $"{Mathf.Clamp(attackTimeDiff, 0f, attackTimeDiff)}");
        }

        // detection state
        Vector3 textPosition = transform.position + (Vector3.up * 1f) + (Vector3.left * 0.5f);
        string message = $"{detectionState} :: {combatData.State}";
        UnityEditor.Handles.Label(textPosition, message);
    }
#endif
}