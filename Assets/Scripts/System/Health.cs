using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class Health : MonoBehaviour
{
    //Player
    [NonSerialized] public PlayerUI playerUI;

    //Enemy
    [Header("EnemyHealthbar")]
    public RectTransform HealthBar;
    [NonSerialized] public Image HealthBarImage;
    public float HealthBarOffset = 1f;

    //Values
    [Header("Values")]
    [SerializeField] private int maxHealth = 1;
    private int currentHealth;
    private int baseHealth;

    [HideInInspector]
    public UnityEvent dieEvent;

    public int Value
    {
        get { return currentHealth; }
        set { currentHealth = Math.Min(Math.Max(0, value), maxHealth); }
    }

    public int MaxValue
    {
        get { return maxHealth; }
        set { maxHealth = Math.Max(0, value); currentHealth = Math.Min(value, currentHealth); }
    }

    void Start()
    {
        baseHealth = maxHealth;

        if (HealthBar != null) HealthBarImage = HealthBar.GetComponent<Image>();
        currentHealth = maxHealth;

        if (gameObject == Player.Instance.gameObject)
        {
            playerUI = GameManager.Instance.playerUI;
            playerUI.HealthUIUpdate(Value, MaxValue);
        }

        EnemyHealthbarUpdate();
    }

    void LateUpdate()
    {
        if (HealthBar == null) return;
        var healthBarRotation = HealthBar.rotation;
        healthBarRotation.SetLookRotation(transform.forward * -1);
        HealthBar.rotation = healthBarRotation;

        var healthBarPosition = HealthBar.position;
        healthBarPosition.x = transform.position.x;
        healthBarPosition.y = transform.position.y + HealthBarOffset;
        healthBarPosition.z = transform.position.z;
        HealthBar.position = healthBarPosition;
    }

    public void TakeDamage(int amount)
    {
        if (amount == 0) return;

        Value -= amount;

        if (gameObject == Player.Instance.gameObject)
        {
            playerUI.HealthUIUpdate(Value, MaxValue);
            //AudioController.Instance.PlaySoundOneshot((int)AudioController.Sounds.);
        }
        else
        {
            EnemyHealthbarUpdate();
        }

        if (Value <= 0)
        {
            StopAllCoroutines();
            dieEvent?.Invoke();
            Destroy(gameObject);
        }
    }
    public void Heal(int amount)
    {
        if (amount == 0) return;

        Value += amount;

        if (gameObject == Player.Instance.gameObject) playerUI.HealthUIUpdate(Value, MaxValue);
        else
        {
            EnemyHealthbarUpdate();
        }
    }
    private void EnemyHealthbarUpdate()
    {
        if (HealthBar != null)
        {
            HealthBar.sizeDelta = new Vector2(2 * ((float)currentHealth / maxHealth), HealthBar.sizeDelta.y);
        }
    }
}
