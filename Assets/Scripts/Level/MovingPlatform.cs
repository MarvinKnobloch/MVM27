using System;
using UnityEngine;

public class MovingPlatform : MonoBehaviour, IActivate
{
    private Rigidbody2D rb;
    private BoxCollider2D boxCollider;
    private BoxCollider2D childBoxCollider;
    private SpriteRenderer spriteRenderer;

    private Vector3 startPosition;
    private Vector3 endPosition;
    private Vector3 currentPosition;

    [SerializeField] private float travelTime;
    private float moveTimer;

    public Vector3 velocity { get; private set; }
    public Vector3 oldPosition { get; private set; }

    [Header("MoveOnEnter")]
    public bool moveOnEnter;
    public bool moveOnEnterDontStop;
    [SerializeField] private GameObject[] linkOtherPlatforms;
    [SerializeField] private bool fastReturn;
    [SerializeField] private float fastTravelTime;

    [Header("BurningPlatform")]
    [SerializeField] private bool burningPlatform;
    [SerializeField] private int burningDamage;

    [Header("Useable")]
    [SerializeField] private bool notUsable;
    private Color usableColor;
    private Color notUsableColor;

    [NonSerialized] public State state;
    private State lastState;

    public enum State
    {
        MoveToEnd,
        MoveToStart,
        DontMoveOnStartPosition,
        DontMoveCurrentPosition,
    }
    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        boxCollider = GetComponent<BoxCollider2D>();
        childBoxCollider = transform.GetChild(0).GetComponent<BoxCollider2D>();
        spriteRenderer = transform.GetChild(0).GetComponent<SpriteRenderer>();

        float xsize = spriteRenderer.size.x -0.8f;
        boxCollider.size = new Vector2(xsize * 0.98f, boxCollider.size.y);  // 0.49f
        childBoxCollider.size = new Vector2(xsize, boxCollider.size.y);

        startPosition = transform.position;
        startPosition.z = 0;
        endPosition = transform.GetChild(1).transform.position;
        endPosition.z = 0;

        oldPosition = transform.position;

        usableColor = spriteRenderer.color;
        Color notUsable = spriteRenderer.color;
        notUsable.a = 0.2f;
        notUsableColor = notUsable;
        SetUsableState();
    }
    private void OnEnable()
    {
        if (moveOnEnter == false) state = State.MoveToEnd;
        else state = State.DontMoveOnStartPosition;
    }
    private void Update()
    {
        velocity = (transform.position - oldPosition) / Time.deltaTime;
        oldPosition = transform.position;
    }
    private void FixedUpdate()
    {
        switch (state)
        {
            default:
            case State.MoveToEnd:
                ToEnd();
                break;
            case State.MoveToStart:
                ToStart();
                break;
            case State.DontMoveOnStartPosition:
                HoldStartPosition();
                break;
            case State.DontMoveCurrentPosition:
                HoldCurrentPosition();
                break;
        }
    }
    private void ToEnd()
    {
        moveTimer += Time.fixedDeltaTime;
        float precentagecomplete = moveTimer / travelTime;
        rb.MovePosition(Vector2.Lerp(startPosition, endPosition, precentagecomplete));
        if (moveTimer >= travelTime)
        {
            state = State.MoveToStart;
            moveTimer = 0;
        }
    }
    void ToStart()
    {
        if (fastReturn == false)
        {
            MoveBack(travelTime);
        }
        else MoveBack(fastTravelTime);

    }
    private void MoveBack(float time)
    {
        moveTimer += Time.fixedDeltaTime;
        float precentagecomplete = moveTimer / time;
        rb.MovePosition(Vector2.Lerp(endPosition, startPosition, precentagecomplete));
        if (moveTimer >= time)
        {
            if (moveOnEnter == false)
            {
                state = State.MoveToEnd;
                moveTimer = 0;
            }
            else
            {
                if (moveOnEnterDontStop == false)
                {
                    state = State.DontMoveOnStartPosition;
                    moveTimer = 0;
                }
                else
                {
                    state = State.MoveToEnd;
                    moveTimer = 0;
                }
            }
        }
    }
    public void CheckLinkedMovement()
    {
        if (linkOtherPlatforms.Length != 0)
        {
            for (int i = 0; i < linkOtherPlatforms.Length; i++)
            {
                linkOtherPlatforms[i].GetComponent<MovingPlatform>().StartLinkedMovement();
            }
            StartLinkedMovement();
        }
        else StartLinkedMovement();
    }
    private void StartLinkedMovement()
    {
        state = State.MoveToEnd;
        moveTimer = 0;
    }
    private void HoldStartPosition()
    {
        rb.transform.position = startPosition;
        rb.linearVelocity = Vector2.zero;
    }
    private void HoldCurrentPosition()
    {
        rb.transform.position = currentPosition;
        rb.linearVelocity = Vector2.zero;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (gameObject.TryGetComponent(out MovingPlatform platform))
            {
                Player player = Player.Instance;
                player.movingPlatform = platform;

                if (platform.moveOnEnter == true)
                {
                    if (platform.state == MovingPlatform.State.DontMoveOnStartPosition) platform.CheckLinkedMovement();
                }

            }

        }
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (burningPlatform == false) return;

        if (collision.CompareTag("Player"))
        {
            if (Player.Instance.currentElementNumber != 1) Player.Instance.health.TakeDamage(burningDamage, false);
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Player player = Player.Instance;

            if (player.movingPlatform != null)
            {
                if (player.movingPlatform.gameObject == gameObject)
                {
                    player.movingPlatform = null;
                    player.rb.gravityScale = player.baseGravityScale;

                }
            }
        }
    }

    public void SetRequirement()
    {
        return;
    }

    public void Activate()
    {
        currentPosition = transform.position;
        lastState = state;
        state = State.DontMoveCurrentPosition;
    }

    public void Deactivate()
    {
        state = lastState;
    }
    public void SwitchPlatformUseabiltity(bool onlyDisable)
    {
        if (onlyDisable)
        {
            notUsable = true;
            SetUsableState();
        }
        else
        {
            notUsable = !notUsable;
            SetUsableState();
        }
    }
    private void SetUsableState()
    {
        if (notUsable)
        {
            boxCollider.enabled = false;
            childBoxCollider.enabled = false;
            spriteRenderer.color = notUsableColor;
        }
        else
        {
            boxCollider.enabled = true;
            childBoxCollider.enabled = true;
            spriteRenderer.color = usableColor;
        }
    }
}
