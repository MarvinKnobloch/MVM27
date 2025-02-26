using System;
using UnityEngine;

public class Collectable : MonoBehaviour
{
    [SerializeField] private CollectValues[] collectValues;

    public enum Currency
    {
        Health,
        Energy,
        Gold,
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
                    case Currency.Gold:
                        GameManager.Instance.playerUI.GoldUpdate(collectValues[i].amount);
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
