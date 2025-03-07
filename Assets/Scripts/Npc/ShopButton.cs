using UnityEngine;
using TMPro;

public class ShopButton : MonoBehaviour
{
    private Shop shop;

    [SerializeField] private int[] upgradeCosts;
    [SerializeField] private int[] upgradeAmount;
    [SerializeField] private Shop.ShopUpgrades upgradeString;
    [SerializeField] private Upgrades.StatsUpgrades upgrade;

    [Space]
    [SerializeField] private string buttonBonusText;
    [SerializeField] private TextMeshProUGUI buttonText;

    private void Awake()
    {
        shop = GetComponentInParent<Shop>();
    }
    private void OnEnable()
    {
        ButtonUpdate();
    }
    public void Buy()
    {
        int costs = upgradeCosts[PlayerPrefs.GetInt(upgradeString.ToString())];
        if (GameManager.Instance.playerCurrency < costs) return;

        Player.Instance.AddStatUpgrade(upgrade, upgradeAmount[PlayerPrefs.GetInt(upgradeString.ToString())]);
        GameManager.Instance.playerUI.PlayerCurrencyUpdate(-costs);

        PlayerPrefs.SetInt(upgradeString.ToString(), PlayerPrefs.GetInt(upgradeString.ToString()) + 1);

        shop.CurrencyUpdate();
        shop.ShopButtonsUpdate();
       
    }
    public void ButtonUpdate()
    {
        if(PlayerPrefs.GetInt(upgradeString.ToString()) < upgradeCosts.Length)
        {
            //buttonText.fontSize = 24;
            buttonText.text = buttonBonusText + ": <color=green>+" + upgradeAmount[PlayerPrefs.GetInt(upgradeString.ToString())] + "</color>\n";

            int costs = upgradeCosts[PlayerPrefs.GetInt(upgradeString.ToString())];
            if (GameManager.Instance.playerCurrency < costs)
            {
                buttonText.text += "Costs: <color=red>" + costs + "</color>";
            }
            else buttonText.text += "Costs: <color=green>" + costs + "</color>";
        }
        else
        {
            buttonText.fontSize = 35;
            buttonText.text = "Sold Out";
        }
    }
}
