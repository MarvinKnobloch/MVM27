using UnityEngine;

public class UnlockAbility : MonoBehaviour, IInteractables
{
    public GameObject interactObj { get => gameObject; }

    [SerializeField] private string actionText;
    public string interactiontext => actionText;

    [SerializeField] private GameManager.AbilityStrings abilityString;
    [SerializeField] private DialogObj dialog;
    [TextArea]
    [SerializeField] private string unlockText;
    [SerializeField] private bool disableOnCollect;

    [Header("ShrineUnique")]
    [SerializeField] private VoidEventChannel shrineEndEvent;
    [SerializeField] private MoveOnInteraction objAfterCollect;

    private void OnEnable()
    {
        if (shrineEndEvent != null)
        {
            shrineEndEvent.OnEventRaised += ActivateMusic;
        }
    }
    private void OnDisable()
    {
        if (shrineEndEvent != null)
        {
            shrineEndEvent.OnEventRaised += ActivateMusic;
        }
    }
    public void Interaction()
    {
        if(PlayerPrefs.GetInt(abilityString.ToString()) == 0)
        {
            PlayerPrefs.SetInt(abilityString.ToString(), 1);
            Player.Instance.PlayerAbilityUpdate();
            Player.Instance.playerInteraction.RemoveInteraction(this);
            Player.Instance.health.Heal(Player.Instance.health.MaxValue);

            GetComponent<CircleCollider2D>().enabled = false;

            if (objAfterCollect != null) objAfterCollect.Deactivate();

            if(dialog != null)
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
                GameManager.Instance.playerUI.dialogBox.GetComponent<DialogBox>().DialogStart(dialog, false);
                GameManager.Instance.playerUI.dialogBox.SetActive(true);
            }
            else GameManager.Instance.playerUI.MessageBoxEnable(unlockText);


            if (disableOnCollect) gameObject.SetActive(false);
        }
    }
    private void ActivateMusic()
    {
        if (shrineEndEvent != null)
        {
            if (abilityString == GameManager.AbilityStrings.FireElement) AudioManager.Instance.StartMusicFadeOut((int)AudioManager.MusicSongs.Tutorial, true, 0.1f, 1);
            if (abilityString == GameManager.AbilityStrings.AirElement) AudioManager.Instance.StartMusicFadeOut((int)AudioManager.MusicSongs.FireArea, true, 0.1f, 1);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && PlayerPrefs.GetInt(abilityString.ToString()) == 0)
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
