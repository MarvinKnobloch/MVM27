using UnityEngine;

public class Switch : MonoBehaviour, IInteractables
{
    public GameObject interactObj { get => gameObject; }

    [SerializeField] private string actionText;
    public string interactiontext => actionText;
    [SerializeField] private bool activ;

    [SerializeField] private GameObject[] objsToControl;

    public void Interaction()
    {
        activ = !activ;

        foreach (GameObject obj in objsToControl)
        {
            if (activ) obj.GetComponent<IActivate>().Activate();
            else obj.GetComponent<IActivate>().Deactivate();
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
