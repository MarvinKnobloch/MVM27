using System;
using UnityEngine;

public class TutorialNpc : MonoBehaviour
{
    [SerializeField] private int TutorialNumber;
    [SerializeField] private float movementSpeed;
    [SerializeField] private Transform moveToPosition;
    [SerializeField] private bool disableAfterMove;

    //Animations
    [NonSerialized] public Animator animator;
    [NonSerialized] public string currentstate;
    private const string idleState = "Idle";
    private const string runState = "Run";

    [SerializeField] private VoidEventChannel introMoveNpc;

    private States state;
    public enum States
    {
        Idle,
        Move,
    }

    private void Awake()
    {
        SwitchToIdle();
        animator = GetComponent<Animator>();
        if(PlayerPrefs.GetInt("TutorialProgress") >= TutorialNumber)
        {
            transform.parent.gameObject.SetActive(false);
        }
    }
    private void OnEnable()
    {
        if(introMoveNpc != null) introMoveNpc.OnEventRaised += SwitchToMove;
    }
    private void OnDisable()
    {
        if (introMoveNpc != null) introMoveNpc.OnEventRaised -= SwitchToMove;
    }
    private void Update()
    {
        switch (state)
        {
            case States.Idle:
                break;
            case States.Move:
                NpcMove();
                break;
        }
    }
    private void NpcMove()
    {
        transform.Translate(transform.right * movementSpeed * Time.deltaTime, Space.World);
        if(Vector2.Distance(transform.position, moveToPosition.position) < 0.5f)
        {
            if (disableAfterMove) gameObject.SetActive(false);
            else SwitchToIdle();
        }
    }
    private void SwitchToIdle()
    {
        state = States.Idle;
        ChangeAnimationState(idleState);
    }
    private void SwitchToMove()
    {
        if (transform.position.x < moveToPosition.position.x)
        {
            Vector3 localScale = new Vector3(1, 1, 1);
            transform.localScale = localScale;
        }
        else
        {
            Vector3 localScale = new Vector3(-1, 1, 1);
            transform.localScale = localScale;
        }

        ChangeAnimationState(runState);
        state = States.Move;
    }
    public void ChangeAnimationState(string newstate)
    {
        if (currentstate == newstate) return;
        currentstate = newstate;
        if (animator == null) return;

        animator.CrossFadeInFixedTime(newstate, 0.1f);
    }
}
