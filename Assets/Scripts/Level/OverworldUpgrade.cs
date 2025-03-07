using UnityEngine;

public class OverworldUpgrade : MonoBehaviour
{
    [SerializeField] private Upgrades.StatsUpgrades stat;
    [SerializeField] private int amount;
    [SerializeField] private int ID;
    [TextArea][SerializeField] private string upgradeText;

    private void Awake()
    {
        if(PlayerPrefs.GetInt("Upgrade" + ID) == 1)
        {
            gameObject.SetActive(false);
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Player.Instance.AddStatUpgrade(stat, amount);
            PlayerPrefs.SetInt("Upgrade" + ID, 1);

            GameManager.Instance.playerUI.MessageBoxEnable(upgradeText + amount + ".");

            gameObject.SetActive(false);
        }
    }
}
