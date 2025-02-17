using System;
using UnityEngine;
using UnityEngine.InputSystem;
public class Player : MonoBehaviour
{
    public static Player Instance;
    private MenuController menuController;

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
    public float dashtime;
    public float dashstrength;
    public int maxdashstacks;
    [NonSerialized] public int currentdashstacks;

    //Animations
    [NonSerialized] public Animator animator;
    public string currentstate;

    private PlayerMovement playerMovement = new PlayerMovement();
    private PlayerCollision playerCollision = new PlayerCollision();

    public States state;
    public enum States
    {
        Ground,
        GroundIntoAir,
        Air,
        Dash,
        Death,
        Emtpy,
    }
    private void Awake()
    {
        controls = Keybindinputmanager.Controls;
        moveInput = controls.Player.Move;
        rb = GetComponent<Rigidbody2D>();
        playerCollider = GetComponent<BoxCollider2D>();

        baseGravityScale = rb.gravityScale;

        playerMovement.player = this;
        playerCollision.player = this;
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
            //controls.Player.Dash.performed += OnDash;
            //controls.Player.Interact.performed += OnCheat;
        }
        else
        {
            controls.Player.Jump.performed -= playerMovement.JumpInput;
            //controls.Player.Dash.performed -= OnDash;
            //controls.Player.Interact.performed -= OnReset;
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

        }
    }
    private void Update()
    {
        if (menuController.gameIsPaused) return;
        ReadMovementInput();
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

        }
    }
    private void ReadMovementInput()
    {
        moveDirection.x = moveInput.ReadValue<Vector2>().x;
    }

    public void SwitchToGround()
    {
        rb.gravityScale = baseGravityScale;
        currentdashstacks = 0;
        currentJumpCount = 0;

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
        if (animator == null) return;

        animator.CrossFadeInFixedTime(newstate, 0.1f);
    }
}
