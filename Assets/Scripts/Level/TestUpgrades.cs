using System;
using UnityEngine;
using static Collectable;

public class TestUpgrades : MonoBehaviour
{
    [SerializeField] private UpgradeValues[] values;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            for (int i = 0; i < values.Length; i++)
            {
                Player.Instance.AddStatUpgrade(values[i].stat, values[i].amount);
            }
            Destroy(gameObject);
        }
    }
    [Serializable]
    public struct UpgradeValues
    {
        public Upgrades.StatsUpgrades stat;
        public int amount;
    }
}
