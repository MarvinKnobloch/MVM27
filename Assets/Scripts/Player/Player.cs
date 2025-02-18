using System;
using System.Collections.Generic;
using NUnit.Framework;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
public class Player : MonoBehaviour
{
    public static Player Instance;
    [NonSerialized] public MenuController menuController;

    private Controls controls;
    private InputAction moveInput;

    [NonSerialized] public Rigidbody2D rb;
    [NonSerialized] public BoxCollider2D playerCollider;

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

    [Header("Dash")]
    public float dashTime;
    public float dashStrength;
    public int maxDashCount;
    [NonSerialized] public int currentDashCount;

    [Header("WallBoost")]
    public bool canWallBoost;
    public bool performedWallBoost;
    public float XWallBoostStrength;
    public float XWallBoostMovement;
    public float YWallBoostStrength;

    [Header("HeavyPunch")]
    public CircleCollider2D heavyPunchCollider;
    public LayerMask heavyPunchLayer;

    //Animations
    [NonSerialized] public Animator animator;
    [NonSerialized] public string currentstate;

    //Interaction
    [NonSerialized] public List<IInteractables> interactables = new List<IInteractables>();
    [NonSerialized] public IInteractables currentInteractable;
    public IInteractables closestInteraction;
    private float checkTimer;
    private float checkInterval = 0.1f;

    private PlayerMovement playerMovement = new PlayerMovement();
    private PlayerCollision playerCollision = new PlayerCollision();
    private PlayerAbilties playerAbilties = new PlayerAbilties();

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

        baseGravityScale = rb.gravityScale;

        playerMovement.player = this;
        playerCollision.player = this;
        playerAbilties.player = this;
    }
    private void Start()
    {
        state = States.Air;
        menuController = GameManager.Instance.menuController;
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
    public void EnableInputs(bool enabled)
    {
        if (enabled && this.enabled)
        {
            controls.Player.Jump.performed += playerMovement.JumpInput;
            controls.Player.Dash.performed += playerMovement.DashInput;
            controls.Player.Interact.performed += InteractInput;
            controls.Player.WallBoost.performed += playerMovement.WallBoostInput;
            controls.Player.HeavyPunch.performed += playerAbilties.HeavyPunshInput;
        }
        else
        {
            controls.Player.Jump.performed -= playerMovement.JumpInput;
            controls.Player.Dash.performed -= playerMovement.DashInput;
            controls.Player.Interact.performed -= InteractInput;
            controls.Player.WallBoost.performed -= playerMovement.WallBoostInput;
            controls.Player.HeavyPunch.performed -= playerAbilties.HeavyPunshInput;
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
                break;
            case States.HeavyPunch:
                break;
        }
    }
    private void Update()
    {
        if (menuController.gameIsPaused) return;
        ReadMovementInput();
        InteractionUpdate();
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
                playerMovement.DashMovement();
                break;
            case States.HeavyPunch:
                playerAbilties.HeavyPunch();
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

        canWallBoost = false;
        performedWallBoost = false;
        XWallBoostMovement = 0;

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
    private void InteractInput(InputAction.CallbackContext ctx)
    {
        if (menuController.gameIsPaused) return;
        if (currentInteractable == null) return;

        bool pressed = ctx.ReadValueAsButton();
        if (pressed)
        {
            switch (state)
            {
                case Player.States.Ground:
                    currentInteractable.Interaction();
                    break;
                case Player.States.GroundIntoAir:
                    currentInteractable.Interaction();
                    break;
                case Player.States.Air:
                    currentInteractable.Interaction();
                    break;
            }
        }
    }
    public void ChangeAnimationState(string newstate)
    {
        if (currentstate == newstate) return;
        currentstate = newstate;
        if (animator == null) return;

        animator.CrossFadeInFixedTime(newstate, 0.1f);
    }
    public void AddInteraction(IInteractables interactable)
    {
        interactables.Add(interactable);
        GetClosestInteraction();
        GameManager.Instance.playerUI.HandleInteractionBox(true);
    }
    public void RemoveInteraction(IInteractables interactable)
    {
        interactables.Remove(interactable);
        InteractionUpdate();
    }
    private void InteractionUpdate()
    {
        {
            if (interactables.Count != 0)
            {
                checkTimer += Time.deltaTime;
                if (checkInterval > checkTimer)
                {
                    checkTimer = 0;
                    GetClosestInteraction();        
                }
            }
            else
            {
                currentInteractable = null;
                GameManager.Instance.playerUI.HandleInteractionBox(false);
            }
        }
    }
    public void GetClosestInteraction()
    {
        float closestDistance = 10f;
        foreach (IInteractables interaction in Player.Instance.interactables)
        {
            float currentDistance;
            currentDistance = Vector3.Distance(Player.Instance.transform.position, interaction.interactObj.transform.position);
            if (currentDistance < closestDistance)
            {
                closestDistance = currentDistance;
                closestInteraction = interaction;
            }
        }
        currentInteractable = closestInteraction;
        GameManager.Instance.playerUI.InteractionTextUpdate(currentInteractable.interactiontext);
    }
}
