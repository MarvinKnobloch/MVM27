using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class Boss : MonoBehaviour
{
    [NonSerialized] public Health health;
    private BoxCollider2D boxCollider2D;
    [SerializeField] private CircleCollider2D handCollider;

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

    [Header("GroundSlam")]
    [SerializeField] private GameObject[] currentPlatforms;
    [SerializeField] private float timeToSpawnPlatforms;

    //Animations
    private Animator animator;
    [NonSerialized] public string currentstate;

    public States state;
    public enum States
    {
        Awake,
        Idle,
        Attack,
        Death,
    }
    private void Awake()
    {
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
    public void AfterAwake()
    {
        boxCollider2D.enabled = true;
        GameManager.Instance.playerUI.ActivateBossHealth();
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

        Debug.Log(collider.Length);
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
    public void GroundSlamStart()
    {
        ChangeAnimationState("Ground");
        state = States.Attack;
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
        state = States.Death;
        ChangeAnimationState("Death");
    }
    public void Death() 
    { 
        //Trigger Event
        Destroy(gameObject); 
    }

}
