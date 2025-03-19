using UnityEngine;

public class NpcDialog : MonoBehaviour, IInteractables
{
    public GameObject interactObj { get => gameObject; }

    [SerializeField] private string actionText;
    public string interactiontext => actionText;

    [SerializeField] private DialogObj dialog;
    [SerializeField] private bool isMerchant;
    public void Interaction()
    {
        if (dialog.pauseGame == false)
        {
            if (dialog.disableInputs == true)
            {
                Player.Instance.rb.linearVelocity = Vector2.zero;
                Player.Instance.SwitchToGround(true);
                Player.Instance.ChangeAnimationState("Idle");
                Player.Instance.state = Player.States.Ground;
            }
        }

        GameManager.Instance.playerUI.dialogBox.GetComponent<DialogBox>().DialogStart(dialog, isMerchant);
        GameManager.Instance.playerUI.dialogBox.SetActive(true);
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
}
