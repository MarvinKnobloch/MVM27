using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class Health : MonoBehaviour
{
    [NonSerialized] public PlayerUI playerUI;



    //Enemy
    [Header("EnemyHealthbar")]
    public GameObject HealthBarBackground;
    private Image HealthBarImage;
    public float HealthBarOffset = 1f;
    [SerializeField] private bool isBoss;

    //Values
    [Header("Values")]
    [SerializeField] private int maxHealth = 1;
    private int currentHealth;
    private int baseHealth;

    [Header("Boss")]

    [SerializeField] private Bosses bossType;
    [NonSerialized] public TutorialBoss tutorialBoss;
    [NonSerialized] public AirBoss airBoss;

    public enum Bosses
    {
        None,
        TutorialBoss,
        AirBoss,
    }

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
        if (HealthBarBackground != null) HealthBarImage = HealthBarBackground.transform.GetChild(0).GetComponent<Image>();

        if (gameObject == Player.Instance.gameObject)
        {
            playerUI = GameManager.Instance.playerUI;
            baseHealth = MaxValue;
            CalculatePlayerHealth();
            Value = MaxValue;
            playerUI.HealthUIUpdate(Value, MaxValue);
        }
        else 
        {
            if (isBoss)
            {
                switch (bossType)
                {
                    case Bosses.TutorialBoss:
                        tutorialBoss = GetComponent<TutorialBoss>();
                        break;
                    case Bosses.AirBoss:
                        airBoss = GetComponent<AirBoss>();
                        break;
                }
                playerUI = GameManager.Instance.playerUI;

            }
            Value = MaxValue;
            EnemyHealthbarUpdate();
        }

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

    public void TakeDamage(int amount, bool dontIgnoreIFrames)
    {
        if (amount == 0) return;
        if (Value <= 0) return;


        if (gameObject == Player.Instance.gameObject)
        {
            if(dontIgnoreIFrames == false) if (Player.Instance.iframesActive) return;

            Value -= amount;
            playerUI.HealthUIUpdate(Value, MaxValue);

            if(Value > 0) Player.Instance.IFramesStart();

            //AudioController.Instance.PlaySoundOneshot((int)AudioController.Sounds.);
        }
        else if (isBoss)
        {

            Value -= amount;
            playerUI.BossHealthUIUpdate(Value, MaxValue);

            switch (bossType)
            {
                case Bosses.TutorialBoss:
                    tutorialBoss.PhaseUpdate(Value, MaxValue);
                    break;
                case Bosses.AirBoss:
                    airBoss.PhaseUpdate(Value, MaxValue);
                    break;
            }

        }
        else
        {
            Value -= amount;
            EnemyHealthbarUpdate();
        }

        if (Value <= 0)
        {
            StopAllCoroutines();
            dieEvent?.Invoke();

            if (gameObject != Player.Instance.gameObject && isBoss == false) Destroy(gameObject);
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
            HealthBarImage.fillAmount = (float)Value / MaxValue;
        }
    }
    public void CalculatePlayerHealth()
    {
        MaxValue = baseHealth + PlayerPrefs.GetInt(Upgrades.StatsUpgrades.BonusHealth.ToString());
        playerUI.HealthUIUpdate(Value, MaxValue);
    }
}
