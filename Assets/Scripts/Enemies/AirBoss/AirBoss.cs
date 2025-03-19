using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class AirBoss : MonoBehaviour
{
    [NonSerialized] public Health health;
    private BoxCollider2D boxCollider2D;
    private BoxCollider2D childCollider;
    private Vector2 childColliderStartSize;

    [Header("BossDialog")]
    [SerializeField] private GameObject triggerZone;
    [SerializeField] private VoidEventChannel cameraDoorsMusic;
    [SerializeField] private MoveOnInteraction rightDoor;
    [SerializeField] private MoveOnInteraction leftDoor;
    [SerializeField] private Transform bossCamera;
    [SerializeField] private VoidEventChannel triggerBoss;
    private Rigidbody2D rb;

    [Header("AttackTimer")]
    [SerializeField] private float attackTimer;
    [SerializeField] private float randomAttackTimer;
    private float finalAttackTime;
    private float timer;

    [Header("Abilities")]
    [SerializeField] private UnityEvent[] abilities;
    [SerializeField] private bool randomAttackOrder;
    private int currentAttack;

    [Header("Phases")]
    [SerializeField]
    [Range(0, 1)] private float[] phasePercentage;
    private int currentPhase;
    [SerializeField] private int phaseBonusCharges;
    [SerializeField] private int phaseBonusChargeSpeed;
    [SerializeField] private int phaseBonusProjectiles;


    //Ohter
    private bool isleft;

    [Header("Charge")]
    [SerializeField] private int chargeAmount;
    private int currentCharge;
    [SerializeField] public int chargeDamage;
    [SerializeField] private float timeToStartCharge;
    [SerializeField] private Transform[] chargeStartPositions;
    private Vector2 chargeDirection;
    [SerializeField] private float chargeSpeed;
    [SerializeField] private float chargeTime;
    [SerializeField] private Transform[] chargeStartEndPositions;
    [SerializeField] private Transform[] chargeEndPositions;
    private Transform chargeEndPosition;
    [SerializeField] private GameObject energyDrop;
    [SerializeField] private Transform energyDropPosition;

    [Header("ProjectileSpawn")]
    [SerializeField] private GameObject projectiles;
    [SerializeField] private int projectileSpawnPhases;
    private int currentProjectilePhase;
    [SerializeField] private float timeBetweenProjectiles;
    [SerializeField] private int projectileSpawnAmount;
    [SerializeField] private float projectileBaseSpeed;
    [SerializeField] private float projectileRandomSpeed;

    [Header("Tornado")]
    [SerializeField] private GameObject tornado;
    [SerializeField] private Transform tornadoSpawnPosition;
    [SerializeField] private float tornadoSpawnTime;
    [SerializeField] private float randomTornadoTime;
    [SerializeField] private float stunDuration;

    [Header("EnemiesPhase")]
    [SerializeField] private GameObject enemiesToSpawn;
    [SerializeField] private Transform[] waypoints;
    //[SerializeField] private Transform enemySpawnPosition;
    [SerializeField] private int enemySpawnPhases;
    private int currentEnemySpawnPhase;
    private Vector2 birdLeftPositionOnEnemySpawn;
    private Vector2 birdRightPositionOnEnemySpawn;
    private Vector2 birdEnemySpawnPosition;
    [SerializeField] private GameObject feathers;
    [SerializeField] private float timeBetweenFeathers;
    [SerializeField] private int feathersAmount;
    [SerializeField] private int feathersAngleSize;
    [SerializeField] private int feahtersRandomAngle;
    [SerializeField] private int feathersPhases;
    private int currentFeathersPhase;
    private bool enemySpawnPhaseDone;

    [Header("Lava")]
    [SerializeField] private GameObject[] lavaForshadowing;
    [SerializeField] private float timeUntilEruption;
    [SerializeField] private GameObject[] lavaStreams;
    private int currentLavaStream;
    [SerializeField] private float timeBetweenStreams;
    [SerializeField] private float lavaDuration;

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
        ProjectilesWait,
        GetToSpawnEnemyPhase,
        EnemySpawn,
        WaitForFeathers,
        ShootFeathers,
        Stunned,
        Death,
    }

    private void Awake()
    {
        if (PlayerPrefs.GetInt(GameManager.OverworldSaveNames.AirBoss.ToString()) == 1)
        {
            triggerZone.SetActive(false);
            gameObject.SetActive(false);
        }

        animator = GetComponent<Animator>();
        boxCollider2D = GetComponent<BoxCollider2D>();
        boxCollider2D.enabled = false;
        childCollider = transform.GetChild(1).gameObject.GetComponent<BoxCollider2D>();
        childColliderStartSize = childCollider.size;
        rb = GetComponent<Rigidbody2D>();

        health = GetComponent<Health>();
        isleft = true;

        birdLeftPositionOnEnemySpawn = (Vector2)chargeEndPositions[0].position + Vector2.up * 7;
        birdRightPositionOnEnemySpawn = (Vector2)chargeEndPositions[1].position + Vector2.up * 7;
    }
    private void Start()
    {
        if (health != null) health.dieEvent.AddListener(OnDeath);
    }
    private void OnEnable()
    {
        cameraDoorsMusic.OnEventRaised += CameraDoorMusic;
        triggerBoss.OnEventRaised += BossStart;
    }
    private void OnDisable()
    {
        cameraDoorsMusic.OnEventRaised += CameraDoorMusic;
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
            case States.ProjectilesWait:
                WaitForNextProjectiles();
                break;
            case States.GetToSpawnEnemyPhase:
                MoveToEnemySpawnPosition();
                break;
            case States.EnemySpawn:
                break;
            case States.WaitForFeathers:
                WaitForShotFeathers();
                break;
            case States.ShootFeathers:
                break;
            case States.Stunned:
                BossIsStunned();
                break;
            case States.Death:
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
                if (currentAttack == 0 && enemySpawnPhaseDone) currentAttack++;

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
            chargeAmount += phaseBonusCharges;
            chargeSpeed += phaseBonusChargeSpeed; 
            projectileSpawnAmount += phaseBonusProjectiles;
            currentPhase++;
        }
    }
    private void CameraDoorMusic()
    {
        GameManager.Instance.ChangeCamera(bossCamera);

        if (leftDoor != null) leftDoor.Activate();
        if (rightDoor != null) rightDoor.Activate();

        AudioManager.Instance.StartMusicFadeOut((int)AudioManager.MusicSongs.Empty, true, 2, 0.01f);
    }
    public void BossStart()
    {
        GameManager.Instance.playerUI.ToggleBossHealth(true);
        GameManager.Instance.playerUI.BossHealthUIUpdate(health.Value, health.MaxValue);

        InvokeRepeating("LavaStreamActivate", 3, timeBetweenStreams);

        SwitchToIdle();

        AudioManager.Instance.SetSong((int)AudioManager.MusicSongs.Boss);
    }
    private void SwitchToIdle()
    {
        timer = 0;
        CalculateFinalAttackTime();
        ChangeAnimationState("Idle");
        state = States.Idle;
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
        CancelInvoke();
        lavaForshadowing[currentLavaStream].SetActive(false);
        lavaStreams[currentLavaStream].SetActive(false);

        GameManager.Instance.playerUI.ToggleBossHealth(false);

        PlayerPrefs.SetInt(GameManager.OverworldSaveNames.AirBoss.ToString(), 1);
        GameManager.Instance.ChangeCamera(Player.Instance.playerCameraFollow);

        AudioManager.Instance.StartMusicFadeOut((int)AudioManager.MusicSongs.Empty, true, 2, 0.01f);
        if (leftDoor != null) leftDoor.Deactivate();

        boxCollider2D.enabled = false;
        rb.gravityScale = 0;
        childCollider.enabled = false;

        ChangeAnimationState("Death");
        state = States.Death;

        AudioManager.Instance.PlayAudioFileOneShot(AudioManager.Instance.airBossSounds[(int)AudioManager.AirBossSounds.AirbossDeath]);
    }
    public void Death()
    {
        //Destroy(gameObject);
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

            if(Player.Instance.EnergyValue < Player.Instance.fireballCosts)
            {
                Instantiate(energyDrop, energyDropPosition.position, Quaternion.identity);
            }
            ChangeAnimationState("Charge");
            state = States.Charge;
        }
    }
    private void SetCharge()
    {
        int startPositionNumber = UnityEngine.Random.Range(0, chargeStartPositions.Length - 1);
        transform.position = chargeStartPositions[startPositionNumber].position;
        chargeDirection = (Player.Instance.transform.position - chargeStartPositions[startPositionNumber].position).normalized;

        if (transform.position.x > Player.Instance.transform.position.x)
        {
            transform.localScale = new Vector3(1, 1, 1);
            transform.right = -chargeDirection;
        }
        else
        { 
            transform.localScale = new Vector3(-1, 1, 1);
            transform.right = chargeDirection;
        }

        childCollider.size = new Vector2(childCollider.size.x, 0.8f);

        timer = 0;

        AudioManager.Instance.PlayAudioFileOneShot(AudioManager.Instance.airBossSounds[(int)AudioManager.AirBossSounds.AirbossSwoop]);
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
                transform.rotation = Quaternion.identity;
                int number = UnityEngine.Random.Range(0, 2);
                if (number == 0)
                {
                    isleft = true;

                    transform.localScale = new Vector3(-1, 1, 1);
                    transform.position = chargeStartEndPositions[0].position;
                    chargeEndPosition = chargeEndPositions[0];
                }
                else
                { 
                    isleft = false;
                    transform.localScale = new Vector3(1, 1, 1);
                    transform.position = chargeStartEndPositions[1].position;
                    chargeEndPosition = chargeEndPositions[1];
                }

                childCollider.size = childColliderStartSize;
                ChangeAnimationState("Idle");
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

        ChangeAnimationState("ProjectileAttack");
        StartCoroutine(TornadoSpawn());

        AudioManager.Instance.PlayAudioFileOneShot(AudioManager.Instance.airBossSounds[(int)AudioManager.AirBossSounds.AirbossShootFeathers]);
    }
    public void SpawnProjectiles()
    {
        for (int i = 0; i < projectileSpawnAmount; i++)
        {
            GameObject proj = Instantiate(projectiles, transform.position, Quaternion.identity);

            int randomAngle = UnityEngine.Random.Range(-25, 25);
            if (isleft) proj.transform.Rotate(0, 0, 15 + randomAngle);
            else proj.transform.Rotate(0, 0, 165 + randomAngle);

            float randomSpeed = UnityEngine.Random.Range(-projectileRandomSpeed, projectileRandomSpeed);
            proj.GetComponent<Projectile>().projectileSpeed = projectileBaseSpeed + randomSpeed;
        }
    }
   public void SwitchToProjectilesWait()
    {
        if (state != States.ProjectileThrow) return;

        currentProjectilePhase++;
        if (currentProjectilePhase < projectileSpawnPhases)
        {
            timer = 0;

            ChangeAnimationState("Idle");
            state = States.ProjectilesWait;
        }
        else SwitchToIdle();
    }
    private void WaitForNextProjectiles()
    {
        timer += Time.deltaTime;
        if(timer >= timeBetweenProjectiles)
        {
            ChangeAnimationState("ProjectileAttack");
            state = States.ProjectileThrow;

            AudioManager.Instance.PlayAudioFileOneShot(AudioManager.Instance.airBossSounds[(int)AudioManager.AirBossSounds.AirbossShootFeathers]);
        }
    }
    IEnumerator TornadoSpawn()
    {
        yield return new WaitForSeconds(tornadoSpawnTime + UnityEngine.Random.Range(-randomTornadoTime, randomTornadoTime));
        GameObject proj = Instantiate(tornado, tornadoSpawnPosition.position, Quaternion.identity);
        if (isleft) proj.transform.Rotate(0, 0, 0);
        else proj.transform.Rotate(0, 0, 180);

        AudioManager.Instance.PlayAudioFileOneShot(AudioManager.Instance.airBossSounds[(int)AudioManager.AirBossSounds.AirbossTornado]);
    }
    public void SetEnemySpawnPhase()
    {
        if (isleft) birdEnemySpawnPosition = birdLeftPositionOnEnemySpawn;
        else birdEnemySpawnPosition = birdRightPositionOnEnemySpawn;

        state = States.GetToSpawnEnemyPhase;
    }
    private void MoveToEnemySpawnPosition()
    {
        var step = 6 * Time.deltaTime;
        transform.position = Vector2.MoveTowards(transform.position, birdEnemySpawnPosition, step);

        if (Vector2.Distance(transform.position, birdEnemySpawnPosition) < 0.2f)
        {
            if(isleft) transform.localScale = new Vector3(-1, 1, 1);
            else transform.localScale = new Vector3(1, 1, 1);

            ChangeAnimationState("SpawnEnemies");
            state = States.EnemySpawn;

            AudioManager.Instance.PlayAudioFileOneShot(AudioManager.Instance.airBossSounds[(int)AudioManager.AirBossSounds.AirbossMinions]);
        }
    }
    public void SpawnEnemies()
    {
        GameObject enemy1 = Instantiate(enemiesToSpawn, tornadoSpawnPosition.position, Quaternion.identity);

        Transform waypoint;
        if (isleft) waypoint = waypoints[1];
        else waypoint = waypoints[0];
        enemy1.GetComponent<ThwomperEnemy>().SetWaypoints(tornadoSpawnPosition, waypoint);

        enemy1.SetActive(true);

        //GameObject enemy2 = Instantiate(enemiesToSpawn, energyDropPosition.position + -Vector3.right * 3, Quaternion.identity);
        //enemy2.GetComponent<CrawlerEnemy>().SetWaypoints(waypoints[1], waypoints[0]);
    }
    public void SwitchToWaitForFeathers()
    {
        timer = 0;
        ChangeAnimationState("Idle");
        state = States.WaitForFeathers;
    }
    private void WaitForShotFeathers()
    {
        timer += Time.deltaTime;
        if (timer >= timeBetweenFeathers)
        {
            ChangeAnimationState("ShotFeathers");
            state = States.ShootFeathers;

            AudioManager.Instance.PlayAudioFileOneShot(AudioManager.Instance.airBossSounds[(int)AudioManager.AirBossSounds.AirbossShootFeathers]);
        }
    }
    public void ShotFeathers()
    {
        int angleBetweenFeathers = Mathf.RoundToInt(feathersAngleSize / feathersAmount);
        int startangle = UnityEngine.Random.Range(-feahtersRandomAngle, feahtersRandomAngle);

        if (isleft) startangle = -15 - startangle;
        else startangle =  195 + startangle;

        for (int i = 0; i < feathersAmount; i++)
        {
            GameObject feather = Instantiate(feathers, transform.position, Quaternion.identity);

            feather.transform.Rotate(0, 0, startangle);

            if (isleft) startangle -= angleBetweenFeathers;
            else startangle += angleBetweenFeathers;

        }
    }
    public void ShotFeathersEnd()
    {
        //if (state != States.SpawnFeathers) return;

        currentFeathersPhase++;

        if (currentFeathersPhase < feathersPhases)
        {
            timer = 0;
            SwitchToWaitForFeathers();
        }
        else
        {
            currentEnemySpawnPhase++;

            if (currentEnemySpawnPhase < enemySpawnPhases)
            {
                currentFeathersPhase = 0;
                isleft = !isleft;
                ChangeAnimationState("Idle");
                SetEnemySpawnPhase();
            }
            else
            {
                enemySpawnPhaseDone = true;
                currentFeathersPhase = 0;
                currentEnemySpawnPhase = 0;
                SwitchToIdle();
            }
        }
    }    
    public void GotHitByReflect()
    {
        StopCoroutine("ThrowProjectiles");
        timer = 0;
        boxCollider2D.enabled = true;
        rb.gravityScale = 2;

        ChangeAnimationState("Stun");
        state = States.Stunned;

        AudioManager.Instance.PlayAudioFileOneShot(AudioManager.Instance.airBossSounds[(int)AudioManager.AirBossSounds.AirbossDeath]);
    }
    private void BossIsStunned()
    {
        timer += Time.deltaTime;
        if(timer > stunDuration)
        {
            rb.gravityScale = 0;
            boxCollider2D.enabled = false;

            enemySpawnPhaseDone = false;
            CalculateFinalAttackTime();
            timer = finalAttackTime;
            ChangeAnimationState("Idle");
            state = States.Idle;
        }
    }
    private void LavaStreamActivate()
    {
        currentLavaStream = UnityEngine.Random.Range(0, 2);
        lavaForshadowing[currentLavaStream].SetActive(true);
        StartCoroutine(LavaEruption());
    }
    IEnumerator LavaEruption()
    {
        yield return new WaitForSeconds(timeUntilEruption);
        lavaForshadowing[currentLavaStream].SetActive(false);
        lavaStreams[currentLavaStream].SetActive(true);
        StartCoroutine(LavaStreamDeactivate());
    }
    IEnumerator LavaStreamDeactivate()
    {
        yield return new WaitForSeconds(lavaDuration);
        lavaStreams[currentLavaStream].SetActive(false);
    }

    public void PlayIdleSound()
    {
        switch (state)
        {
            case States.GetToSpawnEnemyPhase:
                AudioManager.Instance.PlayAudioFileOneShot(AudioManager.Instance.airBossSounds[UnityEngine.Random.Range(0, 3)]);
                break;
            case States.ChargeStart:
                AudioManager.Instance.PlayAudioFileOneShot(AudioManager.Instance.airBossSounds[UnityEngine.Random.Range(0, 3)]);
                break;
            case States.ChargeEnd:
                AudioManager.Instance.PlayAudioFileOneShot(AudioManager.Instance.airBossSounds[UnityEngine.Random.Range(0, 3)]);
                break;
                //case States.ProjectilesWait:
                //    AudioManager.Instance.PlayAudioFileOneShot(AudioManager.Instance.airBossSounds[UnityEngine.Random.Range(0, 3)]);
                //    break;
        }
    }
}
