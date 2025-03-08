using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class TutorialBoss : MonoBehaviour
{
    [NonSerialized] public Health health;
    private BoxCollider2D boxCollider2D;
    private CircleCollider2D handCollider;
    [SerializeField] private VoidEventChannel killHelper;
    [SerializeField] private VoidEventChannel triggerBoss;

    [Header("AttackTimer")]
    [SerializeField] private float attackTimer;
    [SerializeField] private float randomAttackTimer;
    private float finalAttackTime;
    private float timer;

    [Header("Damage")]
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private int handDamage;

    [Header("Abilities")]
    [SerializeField] private UnityEvent[] abilities;
    [SerializeField] private bool randomAttackOrder;
    private int currentAttack;

    [Header("Phases")]
    [SerializeField]
    [Range(0, 1)] private float[] phasePercentage;
    private int currentPhase;

    [Header("AirSlam")]
    [SerializeField] private GameObject airSlamProjectile;
    [SerializeField] private int projectileAmount;
    [SerializeField] private Transform airSlamProjectileSpawn;
    [SerializeField] private float spawnSpreading;
    [SerializeField] private int projectileWaves;
    [SerializeField] private int currentProjectileWave;
    [SerializeField] private float projectileSpawnInterval;

    [Header("GroundSlam")]
    [SerializeField] private GameObject[] currentPlatforms;
    [SerializeField] private float timeToSpawnPlatforms;
    [SerializeField] private GameObject fireZone;
    [SerializeField] private float fireZoneLifetime;

    [Header("Death")]
    [SerializeField] private GameObject fireUpgrade;

    //Animations
    private Animator animator;
    [NonSerialized] public string currentstate;

    public States state;
    public enum States
    {
        Wait,
        Idle,
        Attack,
        Death,
    }

    private void Awake()
    {
        if (PlayerPrefs.GetInt("TutorialBoss") == 1) gameObject.SetActive(false);

        animator = GetComponent<Animator>();
        boxCollider2D = GetComponent<BoxCollider2D>();
        boxCollider2D.enabled = false;
        handCollider = GetComponentInChildren<CircleCollider2D>();

        health = GetComponent<Health>();
    }
    private void Start()
    {
        if (health != null) health.dieEvent.AddListener(OnDeath);
    }
    private void OnEnable()
    {
        killHelper.OnEventRaised += KillHelper;
        triggerBoss.OnEventRaised += BossStart;
    }
    private void OnDisable()
    {
        killHelper.OnEventRaised -= KillHelper;
        triggerBoss.OnEventRaised -= BossStart;
    }

    void Update()
    {
        switch (state)
        {
            case States.Idle:
                WaitForAttack();
                break;
            case States.Attack:
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
        if(percentage <= phasePercentage[currentPhase])
        {
            SwitchToIdle();
            StopCoroutine("DisableFireZone");
            fireZone.SetActive(false);
            currentPlatforms[currentPhase].SetActive(false);
            currentPhase++;
            StartCoroutine(ActivateNewPlatforms());
        }
    }
    IEnumerator ActivateNewPlatforms()
    {
        yield return new WaitForSeconds(timeToSpawnPlatforms);
        currentPlatforms[currentPhase].SetActive(true);
    }
    public void BossStart()
    {
        boxCollider2D.enabled = true;
        GameManager.Instance.playerUI.ToggleBossHealth(true);
        GameManager.Instance.playerUI.BossHealthUIUpdate(health.Value, health.MaxValue);

        currentPlatforms[currentPhase].SetActive(true);
        SwitchToIdle();
    }
    private void SwitchToIdle()
    {
        timer = 0;
        CalculateFinalAttackTime();
        ChangeAnimationState("Idle");
        state = States.Idle;
    }
    public void DealHandDamage()
    {
        Collider2D[] collider = Physics2D.OverlapCircleAll(handCollider.bounds.center, handCollider.radius, playerLayer);

        foreach (Collider2D col in collider)
        {
            if (col.TryGetComponent(out Health health))
            {
                health.TakeDamage(handDamage, false);
            }
        }
    }
    public void AttackEnd()
    {
        if (state == States.Attack)
        {
            SwitchToIdle();
        }
    }
    public void AirSlamStart()
    {
        ChangeAnimationState("Air");
        state = States.Attack;
    }
    public void StartAirProjectileSpawn()
    {
        currentProjectileWave = 0;
        StartCoroutine(SpawnAirSlamProjectiles());
    }
    IEnumerator SpawnAirSlamProjectiles()
    {
        while (currentProjectileWave < projectileWaves)
        {
            currentProjectileWave++;
            for (int i = 0; i < projectileAmount; i++)
            {
                float randomSpawnPoistion = UnityEngine.Random.Range(-spawnSpreading, spawnSpreading);
                Instantiate(airSlamProjectile, new Vector3(airSlamProjectileSpawn.transform.position.x + randomSpawnPoistion, airSlamProjectileSpawn.transform.position.y, 0), Quaternion.identity);
            }
            yield return new WaitForSeconds(projectileSpawnInterval);
        }
    }

    public void GroundSlamStart()
    {
        ChangeAnimationState("Ground");
        state = States.Attack;
    }
    public void ActivateFireZone()
    {
        fireZone.SetActive(true);
        StartCoroutine("DisableFireZone");
    }
    IEnumerator DisableFireZone()
    {
        yield return new WaitForSeconds(fireZoneLifetime);
        fireZone.SetActive(false);
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
        fireZone.SetActive(false);

        PlayerPrefs.SetInt("TutorialProgress", PlayerPrefs.GetInt("TutorialProgress") + 1);
        PlayerPrefs.SetInt("TutorialBoss", 1);

        StopAllCoroutines();
        state = States.Death;
        ChangeAnimationState("Death");
    }
    public void Death() 
    {
        fireUpgrade.SetActive(true);
        GameManager.Instance.playerUI.ToggleBossHealth(false);
        Destroy(gameObject); 
    }

    public void KillHelper()
    {
        ChangeAnimationState("Kill");
    }
    public void IdleAfterHelperKill()
    {
        ChangeAnimationState("Idle");
    }

}
