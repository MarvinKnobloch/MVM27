using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class AirBoss : MonoBehaviour
{
    [NonSerialized] public Health health;
    private BoxCollider2D boxCollider2D;
    [SerializeField] private VoidEventChannel triggerBoss;

    [Header("AttackTimer")]
    [SerializeField] private float attackTimer;
    [SerializeField] private float randomAttackTimer;
    private float finalAttackTime;
    private float timer;

    [Header("Damage")]
    [SerializeField] private LayerMask playerLayer;

    [Header("Abilities")]
    [SerializeField] private UnityEvent[] abilities;
    [SerializeField] private bool randomAttackOrder;
    private int currentAttack;

    [Header("Phases")]
    [SerializeField]
    [Range(0, 1)] private float[] phasePercentage;
    private int currentPhase;

    //Ohter
    private bool isleft;

    [Header("Charge")]
    [SerializeField] private int chargeAmount;
    private int currentCharge;
    [SerializeField] private int chargeDamage;
    [SerializeField] private float timeToStartCharge;
    [SerializeField] private Transform[] chargeStartPositions;
    private Vector2 chargeDirection;
    [SerializeField] private float chargeSpeed;
    [SerializeField] private float chargeTime;
    [SerializeField] private Transform[] chargeEndPositions;
    private Transform chargeEndPosition;

    [Header("ProjectileSpawn")]
    [SerializeField] private GameObject projectiles;
    [SerializeField] private int projectileSpawnPhases;
    private int currentProjectilePhase;
    [SerializeField] private int timeBetweenProjectiles;
    [SerializeField] private int projectileSpawnAmount;
    [SerializeField] private float projectileBaseSpeed;
    [SerializeField] private float projectileRandomSpeed;

    [Header("Tornado")]
    [SerializeField] private GameObject tornado;
    [SerializeField] private Transform tornadoSpawnPosition;
    //Animations
    private Animator animator;
    [NonSerialized] public string currentstate;

    public States state;
    public enum States
    {
        Wait,
        Idle,
        ChargeStart,
        Charge,
        ChargeEnd,
        ProjectileThrow,
        Death,
    }

    private void Awake()
    {
        animator = GetComponent<Animator>();
        boxCollider2D = GetComponent<BoxCollider2D>();
        boxCollider2D.enabled = false;

        health = GetComponent<Health>();
        isleft = false;
    }
    private void Start()
    {
        if (health != null) health.dieEvent.AddListener(OnDeath);
    }
    private void OnEnable()
    {
        triggerBoss.OnEventRaised += BossStart;
    }
    private void OnDisable()
    {
        triggerBoss.OnEventRaised -= BossStart;
    }

    void Update()
    {
        switch (state)
        {
            case States.Idle:
                WaitForAttack();
                break;
            case States.ChargeStart:
                MoveOutOfScreen();
                break;
            case States.Charge:
                ChargeMovement();
                break;
            case States.ChargeEnd:
                MoveIntoScreen();
                break;
        }
    }
    private void CalculateFinalAttackTime()
    {
        finalAttackTime = attackTimer + UnityEngine.Random.Range(-randomAttackTimer, randomAttackTimer);
    }

    private void WaitForAttack()
    {
        timer += Time.deltaTime;
        if (timer >= finalAttackTime)
        {
            if (randomAttackOrder)
            {

            }
            else
            {
                abilities[currentAttack].Invoke();
                if (currentAttack < abilities.Length - 1) currentAttack++;
                else currentAttack = 0;
            }

        }
    }
    public void PhaseUpdate(int current, int max)
    {
        if (currentPhase == phasePercentage.Length) return;

        float percentage = (float)current / max;
        if (percentage <= phasePercentage[currentPhase])
        {
            currentPhase++;
        }
    }
    public void BossStart()
    {
        boxCollider2D.enabled = true;
        GameManager.Instance.playerUI.ActivateBossHealth();
        GameManager.Instance.playerUI.BossHealthUIUpdate(health.Value, health.MaxValue);

        SwitchToIdle();
    }
    private void SwitchToIdle()
    {
        timer = 0;
        CalculateFinalAttackTime();
        ChangeAnimationState("Idle");
        state = States.Idle;
    }
    public void AttackEnd()
    {
        //if (state == States.Attack)
        //{
        //    SwitchToIdle();
        //}
    }

    public void ChangeAnimationState(string newstate)
    {
        if (currentstate == newstate) return;
        currentstate = newstate;
        if (animator == null) return;

        animator.CrossFadeInFixedTime(newstate, 0.1f);
    }
    private void OnDeath()
    {
        StopAllCoroutines();
        state = States.Death;
        ChangeAnimationState("Death");
    }
    public void Death()
    {
        //Trigger Event
        Destroy(gameObject);
    }

    public void StartChargeAbility()
    {
        currentCharge = 0;
        timer = 0;
        state = States.ChargeStart;
    }
    private void MoveOutOfScreen()
    {
        transform.Translate(transform.up * 10 * Time.deltaTime, Space.World);

        timer += Time.deltaTime;
        if(timer > timeToStartCharge)
        {
            SetCharge();

            state = States.Charge;
        }
    }
    private void SetCharge()
    {
        int startPositionNumber = UnityEngine.Random.Range(0, chargeStartPositions.Length - 1);
        transform.position = chargeStartPositions[startPositionNumber].position;
        chargeDirection = (Player.Instance.transform.position - chargeStartPositions[startPositionNumber].position).normalized;

        timer = 0;
    }
    private void ChargeMovement()
    {
        transform.Translate(chargeDirection * chargeSpeed * Time.deltaTime, Space.World);

        timer += Time.deltaTime;
        if (timer > chargeTime)
        {
            currentCharge++;
            if (currentCharge >= chargeAmount)
            {
                int number = UnityEngine.Random.Range(0, 2);
                if (number == 0)
                {
                    isleft = true;
                    transform.localScale = new Vector3(-1, 1, 1);
                    transform.position = chargeStartPositions[0].position;
                    chargeEndPosition = chargeEndPositions[0];
                }
                else
                { 
                    isleft = false;
                    transform.localScale = new Vector3(1, 1, 1);
                    transform.position = chargeStartPositions[1].position;
                    chargeEndPosition = chargeEndPositions[1];
                }
                state = States.ChargeEnd;
            }
            else
            {
                SetCharge();
            }
        }
    }
    private void MoveIntoScreen()
    {
        var step = 8 * Time.deltaTime;
        transform.position = Vector2.MoveTowards(transform.position, chargeEndPosition.position, step);
        //transform.Translate(transform.right * 6 * Time.deltaTime, Space.World);

        if(Vector2.Distance(transform.position, chargeEndPosition.position) < 0.5f)
        {
            SwitchToIdle();
        }
    }
    public void StartProjectileSpawn()
    {
        currentProjectilePhase = 0;
        state = States.ProjectileThrow;
        StartCoroutine(ThrowProjectiles());
    }
    IEnumerator ThrowProjectiles()
    {
        while (currentProjectilePhase < projectileSpawnPhases)
        {
            currentProjectilePhase++;
            SpawnProjectiles();
            yield return new WaitForSeconds(timeBetweenProjectiles);
        }
        SwitchToIdle();
    }
    private void SpawnProjectiles()
    {
        for (int i = 0; i < projectileSpawnAmount; i++)
        {
            GameObject proj = Instantiate(projectiles, transform.position, Quaternion.identity);

            int randomAngle = UnityEngine.Random.Range(-20, 20);
            if (isleft) proj.transform.Rotate(0, 0, 15 + randomAngle);
            else proj.transform.Rotate(0, 0, 165 + randomAngle);

            float randomSpeed = UnityEngine.Random.Range(-projectileRandomSpeed, projectileRandomSpeed);
            proj.GetComponent<Projectile>().projectileSpeed = projectileBaseSpeed + randomSpeed;
        }
    }
    IEnumerator TornadoSpawn()
    {
        yield return new WaitForSeconds(2 + currentPhase);
        GameObject proj = Instantiate(tornado, tornadoSpawnPosition.position, Quaternion.identity);
        if (isleft) proj.transform.Rotate(0, 0, 180);
    }
}
