using System;
using Unity.Cinemachine;
using UnityEngine;

public class PermaButton : MonoBehaviour, IInteractables
{
    public GameObject interactObj { get => gameObject; }

    [SerializeField] private string actionText;
    public string interactiontext => actionText;

    [SerializeField] private GameObject[] objsToControl;
    [SerializeField] private GameManager.OverworldSaveNames saveName;

    [Space]
    [TextArea]
    [SerializeField] private string unlockText;

    private CircleCollider2D circleCollider;
    private Animator animator;
    private string currentstate;

    void Start()
    {
        circleCollider = GetComponent<CircleCollider2D>();
        animator = GetComponent<Animator>();
        if (saveName != GameManager.OverworldSaveNames.Empty)
        {
            if (GameManager.Instance.LoadProgress(saveName) == true) Interaction();
        }
    }
    public void Interaction()
    {
        foreach (GameObject obj in objsToControl)
        {
            obj.GetComponent<IActivate>().Activate();
        }
        circleCollider.enabled = false;

        ChangeAnimationState("Pressed");
        if(GameManager.Instance.LoadProgress(saveName) == false)
        {
            if (unlockText != string.Empty) GameManager.Instance.playerUI.MessageBoxEnable(unlockText);
        }

        Player.Instance.playerInteraction.RemoveInteraction(this);
        GameManager.Instance.SaveProgress(saveName);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Player.Instance.playerInteraction.AddInteraction(this);
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Player.Instance.playerInteraction.RemoveInteraction(this);
        }
    }
    public void ChangeAnimationState(string newstate)
    {
        if (currentstate == newstate) return;
        currentstate = newstate;
        if (animator == null) return;

        animator.CrossFadeInFixedTime(newstate, 0.1f);
    }
}
