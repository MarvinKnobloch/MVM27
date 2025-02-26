using System;
using System.Collections.Generic;
using NUnit.Framework;
using TMPro;
using UnityEditor.Playables;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
public class Player : MonoBehaviour
{
    public static Player Instance;
    [NonSerialized] public MenuController menuController;
    [NonSerialized] public PlayerUI playerUI;

    private Controls controls;
    private InputAction moveInput;

    [NonSerialized] public Rigidbody2D rb;
    [NonSerialized] public BoxCollider2D playerCollider;
    [NonSerialized] public Health health;

    [Header("Movement")]
    public float movementSpeed;
    public float jumpStrength;
    public int maxJumpCount;
    public int maxFallSpeed;
    public float groundIntoAirOffset;
    [NonSerialized] public float groundIntoAirTimer;
    [NonSerialized] public int currentJumpCount;
    [NonSerialized] public Vector2 moveDirection;
    [NonSerialized] public Vector2 playerVelocity;
    [NonSerialized] public bool faceRight;
    [NonSerialized] public float baseGravityScale;
    public LayerMask groundCheckLayer;
    [NonSerialized] public float sidewardsStreamMovement;

    [Header("Dash")]
    public float dashTime;
    public float dashStrength;
    public int maxDashCount;
    [NonSerialized] public int currentDashCount;

    [Header("Other")]
    public Transform projectileSpawnPosition;

    [Header("Energy")]
    [SerializeField] private int maxEnergy;
    private int currentEnergy;
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
    public CircleCollider2D heavyPunchCollider;
    public LayerMask heavyPunchLayer;

    [Header("Fireball")]
    public GameObject fireballPrefab;
    public float fireballCastTime;
    public int fireballCosts;

    [Header("NonElementHeal")]
    public int elementHealAmount;
    public int elementHealCosts;

    [Header("ElementalSwitch")]
    public GameObject[] elementalSprite;
    [NonSerialized] public int currentElementNumber;

    //Animations
    public Animator[] elementalAnimator;
    [NonSerialized] public Animator currentAnimator;
    [NonSerialized] public string currentstate;
    const string deathState = "PlayerDeath";

    //Interaction
    [NonSerialized] public List<IInteractables> interactables = new List<IInteractables>();
    [NonSerialized] public IInteractables currentInteractable;
    public IInteractables closestInteraction;

    //AbilitiesUnlocked
    public bool fireElementUnlocked;
    [NonSerialized] public bool fireBallUnlocked;
    [NonSerialized] public bool wallbreakUnlocked;
    [NonSerialized] public bool airElementUnlocked;
    [NonSerialized] public bool doubleJumpUnlocked;
    [NonSerialized] public bool wallBoostUnlocked;

    private PlayerAttack playerAttack;
    [NonSerialized] public PlayerMovement playerMovement = new PlayerMovement();
    private PlayerCollision playerCollision = new PlayerCollision();
    [NonSerialized] public PlayerAbilties playerAbilties = new PlayerAbilties();
    [NonSerialized] public PlayerInteraction playerInteraction = new PlayerInteraction();

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
        Emtpy,
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

        currentAnimator = elementalAnimator[0];
        baseGravityScale = rb.gravityScale;

        playerAttack = GetComponent<PlayerAttack>();
        playerMovement.player = this;
        playerCollision.player = this;
        playerAbilties.player = this;
        playerInteraction.player = this;

    }
    private void Start()
    {
        menuController = GameManager.Instance.menuController;
        playerUI = GameManager.Instance.playerUI;

        EnergyUpdate(Mathf.RoundToInt(EnergyMaxValue * 0.5f));

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
        if (PlayerPrefs.GetInt(GameManager.AbilityStrings.PlayerDoubleJump.ToString()) == 1) doubleJumpUnlocked = true;
        else doubleJumpUnlocked = false;
        if (PlayerPrefs.GetInt(GameManager.AbilityStrings.WallBoost.ToString()) == 1) wallBoostUnlocked = true;
        else wallBoostUnlocked = false;
    }
    private void CheckAbilityState(string pref, bool ability)
    {
        Debug.Log(PlayerPrefs.GetInt(pref.ToString()));
        if (PlayerPrefs.GetInt(pref.ToString()) == 1) ability = true;
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
                break;
            case States.NonElementalHeal:
                break;
            case States.Attack:
                break;
            case States.FireBall:
                break;
        }
    }
    private void Update()
    {
        if (controls.Player.ElementAbility2.WasPerformedThisFrame()) health.TakeDamage(1);

        if (menuController.gameIsPaused) return;
        ReadMovementInput();
        playerInteraction.InteractionUpdate();
        switch (state)
        {
            case States.Emtpy:
                break;
            case States.Ground:
                playerCollision.GroundCheck();
                playerMovement.RotatePlayer();
                break;
            case States.GroundIntoAir:
                playerMovement.GroundIntoAirTransition();
                playerCollision.AirCheck();
                playerMovement.RotatePlayer();
                break;
            case States.Air:
                playerCollision.AirCheck();
                playerMovement.RotatePlayer();
                break;
            case States.Dash:
                playerMovement.DashTime();
                break;
            case States.HeavyPunch:
                playerAbilties.HeavyPunch();
                break;
            case States.NonElementalHeal:
                break;
            case States.FireBall:
                playerAbilties.CastFireball();
                break;
        }
    }
    private void ReadMovementInput()
    {
        moveDirection.x = moveInput.ReadValue<Vector2>().x;
    }

    public void SwitchToGround()
    {
        rb.gravityScale = baseGravityScale;
        currentDashCount = 0;
        currentJumpCount = 0;
        XWallBoostMovement = 0;
        sidewardsStreamMovement = 0;

        playerAttack.airAttackPerformed = false;
        canWallBoost = false;
        performedWallBoost = false;

        state = States.Ground;
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
    public void CreatePrefab(GameObject obj, Transform spawnPosition)
    {
        GameObject projectile = Instantiate(obj, spawnPosition.position, Quaternion.identity);
        if (faceRight) projectile.transform.Rotate(0, 180, 0);
    }
    public void EnergyUpdate(int amount)
    {
        EnergyValue += amount;
        playerUI.EnergyUIUpdate(EnergyValue, EnergyMaxValue);
    }
    private void OnDeath()
    {
        //animation
        playerAttack.state = PlayerAttack.States.Empty;
        rb.linearVelocity = Vector2.zero;
        ChangeAnimationState(deathState);
        state = States.Death;

        //trigger GameOver
    }
    public void RestartGame()
    {
        menuController.gameIsPaused = false;
        Time.timeScale = 1;
        SceneManager.LoadScene(PlayerPrefs.GetInt("CurrentLevel"));
    }
}
