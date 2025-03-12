using UnityEngine;

public class OverworldCurrency : MonoBehaviour, IInteractables
{
    [SerializeField] private int amount;
    [SerializeField] private int ID;
    [SerializeField] private string pickUpText;
    [SerializeField] private string secondText;

    [Space]
    [SerializeField] private string actionText;
    public GameObject interactObj { get => gameObject; }
    public string interactiontext => actionText;

    private CircleCollider2D circleCollider;

    private void Awake()
    {
        circleCollider = GetComponent<CircleCollider2D>();

        if (PlayerPrefs.GetInt("Upgrade" + ID) == 1)
        {
            gameObject.SetActive(false);
        }

    }
    public void Interaction()
    {
        GameManager.Instance.playerUI.PlayerCurrencyUpdate(amount);
        PlayerPrefs.SetInt("Upgrade" + ID, 1);

        GameManager.Instance.playerUI.MessageBoxEnable(pickUpText + "<color=green>" + amount + "</color>" + secondText);

        circleCollider.enabled = false;
        Player.Instance.playerInteraction.RemoveInteraction(this);

        gameObject.SetActive(false);
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
