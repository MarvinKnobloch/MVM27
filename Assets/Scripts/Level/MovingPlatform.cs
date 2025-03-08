using System;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    private Rigidbody2D rb;
    private BoxCollider2D boxCollider;

    private Vector3 startPosition;
    private Vector3 endPosition;

    [SerializeField] private float travelTime;
    private float moveTimer;

    public Vector3 velocity { get; private set; }
    public Vector3 oldPosition { get; private set; }

    public bool moveOnEnter;
    public bool moveOnEnterDontStop;
    [SerializeField] private GameObject[] linkOtherPlatforms;
    [SerializeField] private bool fastReturn;
    [SerializeField] private float fastTravelTime;

    [Header("BurningPlatform")]
    [SerializeField] private bool burningPlatform;
    [SerializeField] private int burningDamage;

    [NonSerialized] public State state;

    public enum State
    {
        MoveToEnd,
        MoveToStart,
        DontMove,
    }
    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        boxCollider = GetComponent<BoxCollider2D>();
        float xsize = transform.GetChild(0).GetComponent<SpriteRenderer>().size.x -0.8f;
        boxCollider.size = new Vector2(xsize * 0.98f, boxCollider.size.y);  // 0.49f
        transform.GetChild(0).GetComponent<BoxCollider2D>().size = new Vector2(xsize, boxCollider.size.y);

        startPosition = transform.position;
        startPosition.z = 0;
        endPosition = transform.GetChild(1).transform.position;
        endPosition.z = 0;

        oldPosition = transform.position;

    }
    private void OnEnable()
    {
        if (moveOnEnter == false) state = State.MoveToEnd;
        else state = State.DontMove;
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
            case State.DontMove:
                HoldPosition();
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
                    state = State.DontMove;
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
    private void HoldPosition()
    {
        rb.transform.position = startPosition;
        rb.linearVelocity = Vector2.zero;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (collision.TryGetComponent(out Player player))
            {
                if (gameObject.TryGetComponent(out MovingPlatform platform))
                {
                    player.movingPlatform = platform;
                    player.SwitchToGround(true);

                    if (platform.moveOnEnter == true)
                    {
                        if (platform.state == MovingPlatform.State.DontMove) platform.CheckLinkedMovement();
                    }

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
            if (collision.TryGetComponent(out Player player))
            {
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
    }
}
