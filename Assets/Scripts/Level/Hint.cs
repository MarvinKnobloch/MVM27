using UnityEngine;
using static GameManager;

public class Hint : MonoBehaviour, IInteractables
{
    public GameObject interactObj { get => gameObject; }

    [SerializeField] private string actionText;
    public string interactiontext => actionText;

    [TextArea]
    [SerializeField] private string hintText;

    [SerializeField] private GameManager.OverworldSaveNames unlockString;

    public void Interaction()
    {
        GameManager.Instance.playerUI.MessageBoxEnable(hintText);

        if(unlockString == GameManager.OverworldSaveNames.FireForestMap || unlockString == GameManager.OverworldSaveNames.FactoryMap)
        {
            PlayerPrefs.SetInt(unlockString.ToString(), 1);
        }
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
