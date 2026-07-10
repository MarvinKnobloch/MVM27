using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class Health : MonoBehaviour
{
    [NonSerialized] public PlayerUI playerUI;

    //Enemy
    [Header("EnemyHealthbar")]
    public GameObject HealthBarBackground;
    private Image HealthBarImage;
    [SerializeField] private bool isBoss;
    [Tooltip("Typically we want this false so that we can play a death animation")]
    [FormerlySerializedAs("autoDestoryOnDeath")] // TODO: an old mispelling. Fix and remove
    [SerializeField] private bool autoDestroyOnDeath = false;

    //Values
    [Header("Values")]
    [SerializeField] private int maxHealth = 1;
    private int currentHealth;
    private int baseHealth;

    [Header("Boss")]

    [SerializeField] private Bosses bossType;
    [NonSerialized] public TutorialBoss tutorialBoss;
    [NonSerialized] public AirBoss airBoss;

    [Header("Enemy")]
    [SerializeField] private GameObject[] activatePuzzleObjsOnDeath;


    public enum Bosses
    {
        None,
        TutorialBoss,
        AirBoss,
    }

    [HideInInspector]
    public UnityEvent dieEvent;
    [HideInInspector]
    public UnityEvent hitEvent;

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

    /// <summary>
    /// A variable to alter at runtime to decide if damage is allowed or not.
    /// Example use: Thwomper
    /// </summary>
    public bool AllowDamage { get; set; } = true;

    private PostWwiseEvent _playerAudio;
    void Start()
    {
        _playerAudio = GetComponentInChildren<PostWwiseEvent>();

        if (HealthBarBackground != null)
            HealthBarImage = HealthBarBackground.transform.GetChild(0).GetComponent<Image>();

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
    public void PlayerTakeDamage(int amount, bool dontIgnoreIFrames, bool knockBack)
    {
        if (!AllowDamage)
            return;
        if (amount == 0)
            return;
        if (Value <= 0)
            return;

        if (dontIgnoreIFrames == false)
            if (Player.Instance.iframesActive)
                return;

        Value -= amount;
        playerUI.HealthUIUpdate(Value, MaxValue);


        if (Value > 0)
            Player.Instance.IFramesStart();

  
        if (_playerAudio != null )
        {
            _playerAudio.PlayImpactEvent("PlayHitEvent");
        }
       

        CheckForDeath();

        if (Value > 0 && knockBack)
        {
            Player.Instance.SwitchToGetHitStun();
            //hitEvent?.Invoke();
        }
    }

    public void EnemyTakeDamage(int amount)
    {
        if (!AllowDamage)
            return;
        if (amount == 0)
            return;
        if (Value <= 0)
            return;

        if (isBoss)
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
        CheckForDeath();

        if(Value > 0)
        {
            hitEvent?.Invoke();
        }

    }
    private void CheckForDeath()
    {
        if (Value <= 0)
        {
            StopAllCoroutines();

            if (activatePuzzleObjsOnDeath.Length != 0)
            {
                foreach (GameObject obj in activatePuzzleObjsOnDeath)
                {
                    if (obj.TryGetComponent(out IActivate activate))
                    {
                        activate.Activate();
                    }
                }
            }

            if (gameObject.TryGetComponent(out EnemyDrops enemyDrops)) enemyDrops.DropsOnDeath();


            dieEvent?.Invoke();
            // TODO: This if check was removed in favor of a config boolean. Keeping until I know all assets have been transitioned.
            //if (gameObject != Player.Instance.gameObject && isBoss == false)
            if (autoDestroyOnDeath)
                Destroy(gameObject);
        }
    }
    public void Heal(int amount)
    {
        if (amount == 0)
            return;

        Value += amount;

        if (gameObject == Player.Instance.gameObject)
            playerUI.HealthUIUpdate(Value, MaxValue);
        else
            EnemyHealthbarUpdate();
    }
    private void EnemyHealthbarUpdate()
    {
        if (HealthBarBackground != null)
            HealthBarImage.fillAmount = (float)Value / MaxValue;
    }
    public void CalculatePlayerHealth()
    {
        MaxValue = baseHealth + PlayerPrefs.GetInt(Upgrades.StatsUpgrades.BonusHealth.ToString());
        playerUI.HealthUIUpdate(Value, MaxValue);
    }
}