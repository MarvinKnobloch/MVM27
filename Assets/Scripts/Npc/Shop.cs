using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;

public class Shop : MonoBehaviour
{
    private Controls controls;

    [SerializeField] private TextMeshProUGUI currencyText;
    [SerializeField] private TextMeshProUGUI closeText;

    [SerializeField] private ShopButton[] allShopButtons;
    public enum ShopUpgrades
    {
        ShopBonusHealth,
        ShopBonusHeal,
        ShopBonusEnergy,
        ShopBonusEnergyRecharge,
        ShopBonusAttack,
        ShopBonusSwitchAttack,
    }
    private void Awake()
    {
        controls = Keybindinputmanager.Controls;
    }
    private void Update()
    {
        if (controls.Player.Interact.WasPerformedThisFrame()) ShopDisable();
    }
    private void OnEnable()
    {
        CurrencyUpdate();
        closeText.text = "Close (<color=green>" + controls.Player.Interact.GetBindingDisplayString() + "</color>)";
    }
    public void CurrencyUpdate()
    {
        currencyText.text = "Currency: " + PlayerPrefs.GetInt("PlayerCurrency");
    }
    public void ShopButtonsUpdate()
    {
        foreach (ShopButton obj in allShopButtons)
        {
            obj.ButtonUpdate();
        }
    }
    public void ShopDisable()
    {
        GameManager.Instance.playerUI.DeactivateShop();
    }
}
