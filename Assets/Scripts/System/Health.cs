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
    public GameObject HealthBarBackground;
    public Image HealthBarImage;
    public float HealthBarOffset = 1f;

    //Values
    [Header("Values")]
    [SerializeField] private int maxHealth = 1;
    [SerializeField] private int currentHealth;
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

        if (HealthBarBackground != null) HealthBarImage = HealthBarBackground.transform.GetChild(0).GetComponent<Image>();
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
        if (HealthBarBackground == null) return;
        var healthBarRotation = HealthBarBackground.transform.rotation;
        healthBarRotation.SetLookRotation(transform.forward * -1);
        HealthBarBackground.transform.rotation = healthBarRotation;

        var healthBarPosition = HealthBarBackground.transform.position;
        healthBarPosition.x = transform.position.x;
        healthBarPosition.y = transform.position.y + HealthBarOffset;
        healthBarPosition.z = transform.position.z;
        HealthBarBackground.transform.position = healthBarPosition;
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
        if (HealthBarBackground != null)
        {
            Debug.Log(Value +"/" + MaxValue);
            HealthBarImage.fillAmount = (float)Value / MaxValue;
        }
    }
}
