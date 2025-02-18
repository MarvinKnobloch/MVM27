using System.Collections;
using UnityEngine;

public class MoveOnInteraction : MonoBehaviour, IActivate
{
    private Vector3 startPosi;
    private Vector3 endPosi;

    private int currentGoals;
    public int requiredGoals;
    private bool isactiv;
    [SerializeField] private float moveDuration = 1.0f;
    private float timer;
    private float travelTime;

    [Space]
    [SerializeField] private bool fastBack;
    [SerializeField] private float backDuration;

    private State state;
    public enum State
    {
        dontMove,
        moveToEnd,
        moveToStart,
    }
    private void Awake()
    {
        startPosi = transform.position;
        endPosi = transform.GetChild(1).gameObject.transform.position;
    }
    private void FixedUpdate()
    {
        switch (state)
        {
            case State.dontMove:
                break;
            case State.moveToEnd:
                Move(startPosi, endPosi);
                break;
            case State.moveToStart:
                Move(endPosi, startPosi);
                break;
        }
    }
    private void Start()
    {
        StartCoroutine(StartCheck());
    }
    IEnumerator StartCheck()
    {
        yield return null;
        CheckRequirements();
    }

    public void Activate()
    {
        currentGoals++;

        CheckRequirements();
    }
    public void CheckRequirements()
    {
        if (currentGoals >= requiredGoals && isactiv == false)
        {
            isactiv = true;
            if (timer != 0)
            {
                if (fastBack) timer = backDuration - timer;
                else timer = moveDuration - timer;
            }

            travelTime = moveDuration;
            if (fastBack)
            {
                timer *= moveDuration / backDuration;
            }

            state = State.moveToEnd;
        }
    }

    public void Deactivate()
    {
        if (currentGoals == requiredGoals && isactiv)
        {
            isactiv = false;
            if (timer != 0) timer = moveDuration - timer;

            travelTime = moveDuration;
            if (fastBack)
            {
                travelTime = backDuration;
                timer *= backDuration / moveDuration;
            }

            state = State.moveToStart;
        }
        currentGoals--;
    }
    private void Move(Vector3 start, Vector3 end)
    {
        if (timer < travelTime)
        {
            float time = timer / travelTime;
            transform.position = Vector3.Lerp(start, end, time);
            timer += Time.deltaTime;
        }
        else
        {
            timer = 0;
            state = State.dontMove;
        }
    }

    public void SetRequirement()
    {
        requiredGoals++;
    }
}
