using UnityEngine;

public class PermaButton : MonoBehaviour, IInteractables
{
    public GameObject interactObj { get => gameObject; }

    [SerializeField] private string actionText;
    public string interactiontext => actionText;

    [SerializeField] private GameObject[] objsToControl;

    [SerializeField] private GameManager.OverworldSaveNames saveName;
    private CircleCollider2D circleCollider;

    void Start()
    {
        circleCollider = GetComponent<CircleCollider2D>();
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
        transform.GetChild(0).transform.position = transform.GetChild(1).transform.position;

        Player.Instance.playerInteraction.RemoveInteraction(this);
        GameManager.Instance.SaveProgress(saveName);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Player.Instance.playerInteraction.AddInteraction(this);
        }
        else if (collision.gameObject.layer == LayerMask.NameToLayer("Fireball"))
        {
            Interaction();
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Player.Instance.playerInteraction.RemoveInteraction(this);
        }
    }
}
