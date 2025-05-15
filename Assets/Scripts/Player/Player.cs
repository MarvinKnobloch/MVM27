using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
public class Player : MonoBehaviour
{
    public static Player Instance;
    [NonSerialized] public MenuController menuController;
    [NonSerialized] public PlayerUI playerUI;

    [NonSerialized] public Controls controls;
    private InputAction moveInput;

    [NonSerialized] public Rigidbody2D rb;
    [NonSerialized] public BoxCollider2D playerCollider;
    [NonSerialized] public Health health;

    [Header("Movement")]
    public float movementSpeed;
    public int maxFallSpeed;
    public float groundIntoAirOffset;
    [NonSerialized] public float groundIntoAirTimer;
    [NonSerialized] public Vector2 moveDirection;
    [NonSerialized] public Vector2 playerVelocity;
    [NonSerialized] public bool faceRight;
    [NonSerialized] public float baseGravityScale;
    public LayerMask groundCheckLayer;
    public int playerGroundDrag;
    [NonSerialized] public float sidewardsStreamMovement;
    [NonSerialized] public MovingPlatform movingPlatform;
    [NonSerialized] public bool abilityGroundCheck;

    [Header("Jump")]
    public float jumpStrength;
    public int maxJumpCount;
    [NonSerialized] public int currentJumpCount;
    public float maxJumpTime;
    [NonSerialized] public float jumpTimer;
    [NonSerialized] public bool jumpPerformed;

    [Header("Dash")]
    public float dashTime;
    public float dashStrength;
    public int maxDashCount;
    [NonSerialized] public int currentDashCount;

    [Header("Attack")]
    public bool autoAttackMovement;
    public float attackMovementSpeed;

    [Header("IFrames")]
    public float iFramesDuration;
    private float iFramesTimer;
    [SerializeField] private float iFramesBlinkSpeed;
    private float iFramesBlinkTimer;
    [NonSerialized] public bool iFramesBlink;
    [NonSerialized] public bool iframesActive;

    [Header("GetHitValues")]
    public float hitStunTime;
    [NonSerialized] public float hitStunTimer;
    private CinemachineImpulseSource impulseSource;
    [SerializeField] private float impulseBaseForce;
    [SerializeField] private float randomImpulseForce;
    [SerializeField] private float baseImpulseVelocity;
    [SerializeField] private float randomImpulseVelocity;

    [Header("Other")]
    public Transform projectileSpawnPosition;
    public Transform playerCameraFollow;

    [Header("Energy")]
    [SerializeField] private int maxEnergy;
    private int currentEnergy;
    private int baseEnergy;
    public int EnergyValue
    {
        get { return currentEnergy; }
        set { currentEnergy = Math.Min(Math.Max(0, value), maxEnergy); }
    }

    public int EnergyMaxValue
    {
        get { return maxEnergy; }
        set { maxEnergy = Math.Max(0, value); currentEnergy = Math.Min(value, currentEnergy); }
    }

    [Header("WallBoost")]
    public bool canWallBoost;
    public bool performedWallBoost;
    public float XWallBoostStrength;
    public float XWallBoostMovement;
    public float YWallBoostStrength;

    [Header("HeavyPunch")]
    public int heavyPunchDamage;
    public int heavyPunchCosts;
    public CircleCollider2D heavyPunchCollider;
    public LayerMask heavyPunchLayer;
    [NonSerialized] public bool heavyPunchPerformed;

    [Header("Fireball")]
    public GameObject fireballPrefab;
    public float fireballCastTime;
    public int fireballCosts;

    [Header("NonElementHeal")]
    public int elementHealAmount;
    public int elementHealCosts;

    [Header("ElementalSwitch")]
    public SpriteRenderer[] elementalSprite;
    [NonSerialized] public int currentElementNumber;

    //Animations
    public Animator[] elementalAnimator;
    [NonSerialized] public Animator currentAnimator;
    [NonSerialized] public string currentstate;
    const string deathState = "Death";

    //Interaction
    [NonSerialized] public List<IInteractables> interactables = new List<IInteractables>();
    [NonSerialized] public IInteractables currentInteractable;
    public IInteractables closestInteraction;

    //AbilitiesUnlocked
    [NonSerialized] public bool fireElementUnlocked;
    [NonSerialized] public bool fireBallUnlocked;
    [NonSerialized] public bool wallbreakUnlocked;
    [NonSerialized] public bool airElementUnlocked;
    //[NonSerialized] public bool doubleJumpUnlocked;
    [NonSerialized] public bool wallBoostUnlocked;
    //[NonSerialized] public bool dashUnlocked;

    [NonSerialized] public PlayerAttack playerAttack;
    [NonSerialized] public PlayerMovement playerMovement = new PlayerMovement();
    [NonSerialized] public PlayerCollision playerCollision = new PlayerCollision();
    [NonSerialized] public PlayerAbilties playerAbilties = new PlayerAbilties();
    [NonSerialized] public PlayerInteraction playerInteraction = new PlayerInteraction();
    private PlayerUpgrades playerUpgrades = new PlayerUpgrades();

    [Space]
    public States state;
    public enum States
    {
        Ground,
        GroundIntoAir,
        Air,
        Dash,
        WallBoost,
        Death,
        HeavyPunch,
        NonElementalHeal,
        FireBall,
        Attack,
        GetHit,
        Emtpy,
        MoveTowards,
    }
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else Destroy(gameObject);

        controls = Keybindinputmanager.Controls;
        moveInput = controls.Player.Move;
        rb = GetComponent<Rigidbody2D>();
        playerCollider = GetComponent<BoxCollider2D>();
        health = GetComponent<Health>();
        impulseSource = GetComponent<CinemachineImpulseSource>();

        currentAnimator = elementalAnimator[0];
        baseGravityScale = rb.gravityScale;

        playerAttack = GetComponent<PlayerAttack>();
        playerMovement.player = this;
        playerCollision.player = this;
        playerAbilties.player = this;
        playerInteraction.player = this;
        playerUpgrades.player = this;

    }
    private void Start()
    {
        menuController = GameManager.Instance.menuController;
        playerUI = GameManager.Instance.playerUI;
        playerUI.SetElementalIcon(currentElementNumber);

        baseEnergy = EnergyMaxValue;
        CalculateMaxEnergy();
        EnergyUpdate(Mathf.RoundToInt(EnergyMaxValue * 0.5f + PlayerPrefs.GetInt(Upgrades.StatsUpgrades.BonusEnergyRecharge.ToString())));

        state = States.Air;
        if (health != null) health.dieEvent.AddListener(OnDeath);

        PlayerAbilityUpdate();
    }
    private void OnEnable()
    {
        controls.Enable();
        EnableInputs(true);
    }
    private void OnDisable()
    {
        controls.Disable();
        EnableInputs(false);
    }
    public void PlayerAbilityUpdate()
    {
        if (PlayerPrefs.GetInt(GameManager.AbilityStrings.FireElement.ToString()) == 1) fireElementUnlocked = true;
        else fireElementUnlocked = false;
        if (PlayerPrefs.GetInt(GameManager.AbilityStrings.Fireball.ToString()) == 1) fireBallUnlocked = true;
        else fireBallUnlocked = false;
        if (PlayerPrefs.GetInt(GameManager.AbilityStrings.WallBreak.ToString()) == 1) wallbreakUnlocked = true;
        else wallbreakUnlocked = false;
        if (PlayerPrefs.GetInt(GameManager.AbilityStrings.AirElement.ToString()) == 1) airElementUnlocked = true;
        else airElementUnlocked = false;
        //if (PlayerPrefs.GetInt(GameManager.AbilityStrings.PlayerDoubleJump.ToString()) == 1) doubleJumpUnlocked = true;
        //else doubleJumpUnlocked = false;
        if (PlayerPrefs.GetInt(GameManager.AbilityStrings.WallBoost.ToString()) == 1) wallBoostUnlocked = true;
        else wallBoostUnlocked = false;
        //if (PlayerPrefs.GetInt(GameManager.AbilityStrings.PlayerDash.ToString()) == 1) dashUnlocked = true;
        //else dashUnlocked = false;
    }
    public void EnableInputs(bool enabled)
    {
        if (enabled && this.enabled)
        {
            controls.Player.Jump.performed += playerMovement.JumpInput;
            controls.Player.Dash.performed += playerMovement.DashInput;
            controls.Player.Attack.performed += playerAttack.AttackInput;
            controls.Player.Interact.performed += playerInteraction.InteractInput;
            controls.Player.ElementAbility1.performed += playerAbilties.Ability1Input;
            controls.Player.ElementAbility2.performed += playerAbilties.Ability2Input;
            controls.Player.Element1.performed += playerAbilties.FirstElementInput;
            controls.Player.Element2.performed += playerAbilties.SecondElementInput;
            controls.Player.Element3.performed += playerAbilties.ThirdElementInput;
        }
        else
        {
            controls.Player.Jump.performed -= playerMovement.JumpInput;
            controls.Player.Dash.performed -= playerMovement.DashInput;
            controls.Player.Attack.performed -= playerAttack.AttackInput;
            controls.Player.Interact.performed -= playerInteraction.InteractInput;
            controls.Player.ElementAbility1.performed -= playerAbilties.Ability1Input;
            controls.Player.ElementAbility2.performed -= playerAbilties.Ability2Input;
            controls.Player.Element1.performed -= playerAbilties.FirstElementInput;
            controls.Player.Element2.performed -= playerAbilties.SecondElementInput;
            controls.Player.Element3.performed -= playerAbilties.ThirdElementInput;
        }
    }
    private void FixedUpdate()
    {
        if (menuController.gameIsPaused) return;

        switch (state)
        {
            case States.Emtpy:
                break;
            case States.Ground:
                playerMovement.GroundMovement();
                break;
            case States.GroundIntoAir:
                playerMovement.AirMovement();
                break;
            case States.Air:
                playerMovement.AirMovement();
                break;
            case States.Dash:
                playerMovement.DashMovement();
                break;
            case States.HeavyPunch:
                playerMovement.AttackMovement();
                break;
            case States.NonElementalHeal:
                playerMovement.AbilityMovement();
                break;
            case States.Attack:
                playerMovement.AttackMovement();
                break;
            case States.FireBall:
                playerMovement.AbilityMovement();
                break;
        }
    }
    private void Update()
    {
        //if (controls.Player.ElementAbility2.WasPerformedThisFrame()) health.TakeDamage(1, false);

        if (menuController.gameIsPaused) return;

        ReadMovementInput();
        playerInteraction.InteractionUpdate();

        playerInteraction.ControllerInteractInput();
        playerAbilties.ControllerAbility1Input();
        playerAbilties.ControllerAbility2Input();
        playerMovement.ControllerDashInput();
        switch (state)
        {
            case States.Emtpy:
                break;
            case States.Ground:
                playerMovement.ControllerJumpInput();
                playerAttack.ControllerAttackInput();
                playerAbilties.ControllerElement1Input();
                playerAbilties.ControllerElement2Input();
                playerAbilties.ControllerElement3Input();
                playerCollision.GroundCheck();
                playerMovement.RotatePlayer();
                break;
            case States.GroundIntoAir:
                playerMovement.JumpIsPressed();
                playerMovement.ControllerJumpInput();
                playerAttack.ControllerAttackInput();
                playerAbilties.ControllerElement1Input();
                playerAbilties.ControllerElement2Input();
                playerAbilties.ControllerElement3Input();
                playerMovement.GroundIntoAirTransition();
                playerCollision.AirCheck();
                playerMovement.RotatePlayer();
                break;
            case States.Air:
                playerMovement.JumpIsPressed();
                playerMovement.ControllerJumpInput();
                playerAttack.ControllerAttackInput();
                playerAbilties.ControllerElement1Input();
                playerAbilties.ControllerElement2Input();
                playerAbilties.ControllerElement3Input();
                playerCollision.AirCheck();
                playerMovement.RotatePlayer();
                break;
            case States.Dash:
                playerMovement.DashTime();
                break;
            case States.HeavyPunch:
                break;
            case States.NonElementalHeal:
                playerAbilties.HoldHeal();
                playerMovement.RotatePlayer();
                playerCollision.GroundCheckWhileAbilities();
                break;
            case States.FireBall:
                playerAbilties.CastFireball();
                playerMovement.RotatePlayer();
                playerCollision.GroundCheckWhileAbilities();
                break;
            case States.GetHit:
                playerMovement.PlayerHitStun();
                break;
        }
    }
    private void ReadMovementInput()
    {
        moveDirection.x = moveInput.ReadValue<Vector2>().x;

        if (moveDirection.x == 0)
        {
            moveDirection.x = Input.GetAxis("Horizontal");
        }
    }

    public void SwitchToGround(bool onlyResetValues)
    {
        rb.gravityScale = baseGravityScale;
        currentDashCount = 0;
        currentJumpCount = 0;
        XWallBoostMovement = 0;
        sidewardsStreamMovement = 0;

        jumpPerformed = false;
        playerAttack.airAttackPerformed = false;
        canWallBoost = false;
        performedWallBoost = false;
        heavyPunchPerformed = false;


        if (onlyResetValues == false)
        {
            state = States.Ground;
        }
    }
    public void SwitchGroundIntoAir()
    {
        groundIntoAirTimer = 0;
        state = States.GroundIntoAir;
    }
    public void SwitchToAir()
    {
        if (currentJumpCount == 0) currentJumpCount++;
        rb.gravityScale = baseGravityScale;

        state = States.Air;
    }
    public void ChangeAnimationState(string newstate)
    {
        if (currentstate == newstate) return;
        currentstate = newstate;
        if (currentAnimator == null) return;

        currentAnimator.CrossFadeInFixedTime(newstate, 0.1f);
    }
    public void CreatePrefab(GameObject obj, Transform spawnPosition, Quaternion rotation)
    {
        GameObject prefab = ObjectPooling.SpawnObject(obj, spawnPosition.transform.position, rotation, ObjectPooling.ProjectileType.Player);
        //GameObject projectile = Instantiate(obj, spawnPosition.position, Quaternion.identity);

        if (faceRight) prefab.transform.Rotate(0, 180, 0);
        else prefab.transform.Rotate(0, 0, 0);
    }
    public void CalculateMaxEnergy()
    {
        EnergyMaxValue = baseEnergy + PlayerPrefs.GetInt(Upgrades.StatsUpgrades.BonusEnergy.ToString());
        playerUI.EnergyUIUpdate(EnergyValue, EnergyMaxValue);
    }
    public void EnergyUpdate(int amount)
    {
        EnergyValue += amount;
        playerUI.EnergyUIUpdate(EnergyValue, EnergyMaxValue);
    }
    public void SwitchToGetHitStun()
    {
        rb.gravityScale = baseGravityScale;
        rb.linearVelocity = Vector2.zero;

        float force = impulseBaseForce + UnityEngine.Random.Range(-randomImpulseForce, randomImpulseForce);
        float xVelocity = baseImpulseVelocity + UnityEngine.Random.Range(-randomImpulseVelocity, randomImpulseVelocity);
        float yVelocity = baseImpulseVelocity + UnityEngine.Random.Range(-randomImpulseVelocity, randomImpulseVelocity);
        impulseSource.DefaultVelocity = new Vector3(xVelocity, yVelocity, 0);
        impulseSource.GenerateImpulseWithForce(force);
        hitStunTimer = 0;
        ChangeAnimationState("GetHit");
        state = States.GetHit;

        AudioManager.Instance.StopPlayerAttackSound();
    }
    public void IFramesStart()
    {
        iframesActive = true;
        iFramesTimer = 0;
        iFramesBlink = false;
        iFramesBlinkTimer = 0;
        StartCoroutine(IFrames());
    }
    IEnumerator IFrames()
    {
        while (iFramesTimer < iFramesDuration)
        {
            iFramesTimer += Time.deltaTime;
            iFramesBlinkTimer += Time.deltaTime;

            if (iFramesBlinkTimer >= iFramesBlinkSpeed)
            {
                iFramesBlinkTimer = 0;
                iFramesBlink = !iFramesBlink;

                if (iFramesBlink) elementalSprite[currentElementNumber].color = Color.red;
                else elementalSprite[currentElementNumber].color = Color.white;
            }
            yield return null;
        }

        iFramesBlink = false;
        elementalSprite[currentElementNumber].color = Color.white;
        iframesActive = false;

    }
    public void AddStatUpgrade(Upgrades.StatsUpgrades upgrade, int amount) => playerUpgrades.AddStatUpgrade(upgrade, amount);
    private void OnDeath()
    {
        //animation
        playerAttack.state = PlayerAttack.States.Empty;
        rb.linearVelocity = Vector2.zero;
        ChangeAnimationState(deathState);
        state = States.Death;

        //AudioManager.Instance.PlayAudioFileOneShot(AudioManager.Instance.utilityFiles[(int)AudioManager.UtilitySounds.PlayerDeath]);
    }
    public void RestartGame()
    {
        menuController.ResetPlayer(false);
    }
    public void StopPlayerControls()
    {
        Player.Instance.rb.linearVelocity = Vector2.zero;
        Player.Instance.SwitchToGround(true);
        Player.Instance.ChangeAnimationState("Idle");
        Player.Instance.state = Player.States.Ground;

        GameManager.Instance.menuController.gameIsPaused = true;
    }
    public void StartPlayerControls()
    {
        GameManager.Instance.menuController.gameIsPaused = false;
    }
}
