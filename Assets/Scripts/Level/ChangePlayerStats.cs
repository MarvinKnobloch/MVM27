using System;
using UnityEngine;
using TMPro;

public class ChangePlayerStats : MonoBehaviour
{
    [SerializeField] private Upgrades.StatsUpgrades stat;
    [SerializeField] private int amount = 1;
    [SerializeField] private TextMeshProUGUI statText;

    [SerializeField] private ChangePlayerStats[] allstats;


    private void OnEnable()
    {
        if(statText != null) TextUpdate();
    }
    public void AddStats()
    {
        Player.Instance.AddStatUpgrade(stat, amount);
        TextUpdate();
    }
    public void RemoveStats()
    {
        Player.Instance.AddStatUpgrade(stat, -amount);
        TextUpdate();
    }
    public void TextUpdate()
    {
        Debug.Log("update");
        int count = PlayerPrefs.GetInt(stat.ToString());
        if(count > 0) statText.text = stat.ToString() + " <color=green>" + count + "</color>";
        else if (count == 0) statText.text = stat.ToString() + " <color=white>" + count + "</color>";
        else statText.text = stat.ToString() + " <color=red>" + count + "</color>";
    }
    public void StatsReset()
    {
        PlayerPrefs.SetInt(Upgrades.StatsUpgrades.BonusHealth.ToString(), 0);
        PlayerPrefs.SetInt(Upgrades.StatsUpgrades.BonusHeal.ToString(), 0);
        PlayerPrefs.SetInt(Upgrades.StatsUpgrades.BonusEnergy.ToString(), 0);
        PlayerPrefs.SetInt(Upgrades.StatsUpgrades.BonusEnergyRecharge.ToString(), 0);
        PlayerPrefs.SetInt(Upgrades.StatsUpgrades.BonusAttack.ToString(), 0);
        PlayerPrefs.SetInt(Upgrades.StatsUpgrades.BonusSwitchAttack.ToString(), 0);

        Player.Instance.AddStatUpgrade(Upgrades.StatsUpgrades.BonusHealth, 0);
        Player.Instance.AddStatUpgrade(Upgrades.StatsUpgrades.BonusHeal, 0);
        Player.Instance.AddStatUpgrade(Upgrades.StatsUpgrades.BonusEnergy, 0);
        Player.Instance.AddStatUpgrade(Upgrades.StatsUpgrades.BonusEnergyRecharge, 0);
        Player.Instance.AddStatUpgrade(Upgrades.StatsUpgrades.BonusAttack, 0);
        Player.Instance.AddStatUpgrade(Upgrades.StatsUpgrades.BonusSwitchAttack, 0);

        for (int i = 0; i < allstats.Length; i++)
        {
            allstats[i].TextUpdate();
        }
    }
}
