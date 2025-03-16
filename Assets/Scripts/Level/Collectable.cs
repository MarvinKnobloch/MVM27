using System;
using System.Collections;
using UnityEngine;

public class Collectable : MonoBehaviour
{
    [SerializeField] private float collectDelay;
    [SerializeField] private float xForceOnSpawn;
    [SerializeField] private float yForceOnSpawn;
    [SerializeField] private float randomForce;
    [SerializeField] private CollectValues[] collectValues;

    private Rigidbody2D rb;
    private CircleCollider2D circleCollider;

    public enum Currency
    {
        Health,
        Energy,
        PlayerCurrency,
    }

    private void Awake()
    {
        rb = GetComponentInParent<Rigidbody2D>();
        circleCollider = GetComponent<CircleCollider2D>();
        circleCollider.enabled = false;
    }
    private void OnEnable()
    {
        float randomX = UnityEngine.Random.Range(-randomForce, randomForce);
        float randomY = UnityEngine.Random.Range(-randomForce, randomForce);
        rb.AddForce(new Vector2(xForceOnSpawn + randomX, yForceOnSpawn +randomY), ForceMode2D.Impulse);

        StartCoroutine(ActivateCollectCollider());
    }
    IEnumerator ActivateCollectCollider()
    {
        yield return new WaitForSeconds(collectDelay);
        circleCollider.enabled = true;
    }
    public void SetValue(int amount)
    {
        if (collectValues.Length == 1)
        {
            collectValues[0].amount = amount;
        }
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
                        Player.Instance.EnergyUpdate(collectValues[i].amount + PlayerPrefs.GetInt(Upgrades.StatsUpgrades.BonusEnergyRecharge.ToString()));
                        break;
                    case Currency.PlayerCurrency:
                        GameManager.Instance.playerUI.PlayerCurrencyUpdate(collectValues[i].amount);
                        break;
                }
            }
            Destroy(transform.parent.gameObject);
        }
    }
}
[Serializable]
public struct CollectValues
{
    public Collectable.Currency currency;
    public int amount;
}
