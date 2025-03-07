using System;
using UnityEngine;

public class Collectable : MonoBehaviour
{
    [SerializeField] private CollectValues[] collectValues;

    public enum Currency
    {
        Health,
        Energy,
        PlayerCurrency,
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            for (int i = 0; i < collectValues.Length; i++)
            {
                switch (collectValues[i].currency)
                {
                    case Currency.Health:
                        Player.Instance.health.Heal(collectValues[i].amount);
                        break;
                    case Currency.Energy:
                        Player.Instance.EnergyUpdate(collectValues[i].amount);
                        break;
                    case Currency.PlayerCurrency:
                        GameManager.Instance.playerUI.PlayerCurrencyUpdate(collectValues[i].amount);
                        break;
                }
            }
            Destroy(gameObject);
        }
    }
}
[Serializable]
public struct CollectValues
{
    public Collectable.Currency currency;
    public int amount;
}
