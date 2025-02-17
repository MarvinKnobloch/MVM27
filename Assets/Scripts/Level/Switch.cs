using UnityEngine;

public class Switch : MonoBehaviour, IInteractables
{
    public GameObject interactObj { get => gameObject;}

    [SerializeField] private string actionText;
    public string interactiontext => actionText;
    [SerializeField] private bool activ;

    [SerializeField] private GameObject objToControl;
    private IActivate activateObj;

    private void Awake()
    {
        activateObj = objToControl.GetComponent<IActivate>();
        activateObj.SetRequirement();
    }
    public void Interaction()
    {
        activ = !activ;
        if(activ) activateObj.Activate();
        else activateObj.Deactivate();

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Player.Instance.AddInteraction(this);
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Player.Instance.RemoveInteraction(this);
        }
    }
}
