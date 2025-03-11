using UnityEngine;

public class DialogTriggerZone : MonoBehaviour
{
    [SerializeField] private DialogObj dialog;
    public void Interaction()
    {
        GameManager.Instance.playerUI.dialogBox.GetComponent<DialogBox>().DialogStart(dialog, false);
        GameManager.Instance.playerUI.dialogBox.SetActive(true);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if(dialog.pauseGame == false)
            {
                if(dialog.disableInputs == true)
                {
                    Player.Instance.rb.linearVelocity = Vector2.zero;
                    Player.Instance.SwitchToGround(true);
                    Player.Instance.ChangeAnimationState("Idle");
                    Player.Instance.state = Player.States.Ground;
                }
            }
            GameManager.Instance.playerUI.dialogBox.GetComponent<DialogBox>().DialogStart(dialog, false);
            GameManager.Instance.playerUI.dialogBox.SetActive(true);
            gameObject.SetActive(false);
        }
    }
}
