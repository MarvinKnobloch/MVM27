using UnityEngine;

public class Switch : MonoBehaviour, IInteractables
{
    public GameObject interactObj { get => gameObject; }

    [SerializeField] private string actionText;
    public string interactiontext => actionText;
    private bool activ;

    [SerializeField] private GameObject[] objsToControl;

    private GameObject inactiveSwitchImage;
    private GameObject activeSwitchImage;

    private void Awake()
    {
        inactiveSwitchImage = transform.GetChild(0).gameObject;
        activeSwitchImage = transform.GetChild(1).gameObject;
    }
    public void Interaction()
    {
        activ = !activ;

        foreach (GameObject obj in objsToControl)
        {
            if (activ)
            {
                obj.GetComponent<IActivate>().Activate();
                inactiveSwitchImage.SetActive(false);
                activeSwitchImage.SetActive(true);
            }
            else
            { 
                obj.GetComponent<IActivate>().Deactivate();
                inactiveSwitchImage.SetActive(true);
                activeSwitchImage.SetActive(false);
            }
        }
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
