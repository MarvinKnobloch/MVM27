using System;
using UnityEngine;

public class OverworldUpgrade : MonoBehaviour, IInteractables
{
    [SerializeField] private Upgrades.StatsUpgrades stat;
    [SerializeField] private int amount;
    [SerializeField] private int ID;
    [TextArea][SerializeField] private string upgradeText;
    [SerializeField] private Sprite[] objSprite;

    private CircleCollider2D circleCollider;
    private SpriteRenderer spriteRenderer;


    [Space]
    [SerializeField] private string actionText;
    public GameObject interactObj { get => gameObject; }
    public string interactiontext => actionText;

    private void Awake()
    {
        circleCollider = GetComponent<CircleCollider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        if(PlayerPrefs.GetInt("Upgrade" + ID) == 1)
        {
            Collected();
        }
        else
        {
            spriteRenderer.sprite = objSprite[0];
        }
    }
    public void Interaction()
    {
        Player.Instance.AddStatUpgrade(stat, amount);
        PlayerPrefs.SetInt("Upgrade" + ID, 1);

        GameManager.Instance.playerUI.MessageBoxEnable(upgradeText + amount + ".");

        Collected();
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
    private void Collected()
    {
        spriteRenderer.sprite = objSprite[1];
        circleCollider.enabled = false;
    }
}
