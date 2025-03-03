using UnityEngine;

public class PlayerUpgrades
{
    public Player player;

    public void AddStatUpgrade(Upgrades.StatsUpgrades upgrade, int amount)
    {
        switch (upgrade)
        {
            case Upgrades.StatsUpgrades.BonusHealth:
                PlayerPrefs.SetInt(upgrade.ToString(), StatsUpdate(upgrade, amount));
                player.health.CalculatePlayerHealth();
                break;
            case Upgrades.StatsUpgrades.BonusHeal:
                PlayerPrefs.SetInt(upgrade.ToString(), StatsUpdate(upgrade, amount));
                break;
            case Upgrades.StatsUpgrades.BonusEnergy:
                PlayerPrefs.SetInt(upgrade.ToString(), StatsUpdate(upgrade, amount));
                player.CalculateMaxEnergy();
                break;
            case Upgrades.StatsUpgrades.BonusEnergyRecharge:
                PlayerPrefs.SetInt(upgrade.ToString(), StatsUpdate(upgrade, amount));
                break;
            case Upgrades.StatsUpgrades.BonusAttack:
                PlayerPrefs.SetInt(upgrade.ToString(), StatsUpdate(upgrade, amount));
                player.playerAttack.SetUpgradeDamage();
                break;
            case Upgrades.StatsUpgrades.BonusFinalAttack:
                PlayerPrefs.SetInt(upgrade.ToString(), StatsUpdate(upgrade, amount));
                player.playerAttack.SetUpgradeDamage();
                break;
            case Upgrades.StatsUpgrades.BonusSwitchAttack:
                PlayerPrefs.SetInt(upgrade.ToString(), StatsUpdate(upgrade, amount));
                player.playerAttack.SetUpgradeDamage();
                break;
        }
    }
    private int StatsUpdate(Upgrades.StatsUpgrades upgrade, int amount)
    {
        return PlayerPrefs.GetInt(upgrade.ToString()) + amount;
    }
}
