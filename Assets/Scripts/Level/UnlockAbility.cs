using UnityEngine;

public class UnlockAbility : MonoBehaviour, IInteractables
{
    public GameObject interactObj { get => gameObject; }

    [SerializeField] private string actionText;
    public string interactiontext => actionText;

    [SerializeField] private GameManager.AbilityStrings abilityString;
    [TextArea]
    [SerializeField] private string unlockText;
    [SerializeField] private bool disableOnCollect;

    public void Interaction()
    {
        if(PlayerPrefs.GetInt(abilityString.ToString()) == 0)
        {
            GameManager.Instance.playerUI.MessageBoxEnable(unlockText);
            PlayerPrefs.SetInt(abilityString.ToString(), 1);
            Player.Instance.PlayerAbilityUpdate();
            Player.Instance.playerInteraction.RemoveInteraction(this);
            Player.Instance.health.Heal(Player.Instance.health.MaxValue);

            GetComponent<CircleCollider2D>().enabled = false;

            if(abilityString == GameManager.AbilityStrings.FireElement) AudioManager.Instance.StartMusicFadeOut((int)AudioManager.MusicSongs.Tutorial, true, 0.1f, 1);

            if (disableOnCollect) gameObject.SetActive(false);
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
