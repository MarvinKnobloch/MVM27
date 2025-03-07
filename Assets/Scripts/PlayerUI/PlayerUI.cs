using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using System;
using System.Collections;

public class PlayerUI : MonoBehaviour
{
    private Controls controls;
    [SerializeField] private GameObject DebugMenu;
    [SerializeField] private Image blackScreen;
    private Color blackScreenColor;

    [Header("Interaction")]
    [SerializeField] private GameObject interactionField;
    [SerializeField] private TextMeshProUGUI interactionText;

    [Header("Health")]
    [SerializeField] private Image healthbar;
    [SerializeField] private TextMeshProUGUI healthText;

    [Header("Energy")]
    [SerializeField] private Image energybar;
    [SerializeField] private TextMeshProUGUI energyText;

    [Header("Currency")]
    [SerializeField] private TextMeshProUGUI currencyText;

    [Header("MessageBox")]
    public GameObject messageBox;
    [SerializeField] private TextMeshProUGUI messageBoxText;
    [SerializeField] private TextMeshProUGUI messageBoxCloseText;

    [Header("DialogBox")]
    public GameObject dialogBox;

    [Header("Intro")]
    [SerializeField] private DialogObj introDialog;
    [SerializeField] private VoidEventChannel disableBlackScreen;
    [SerializeField] private VoidEventChannel standUp;
    [SerializeField] private VoidEventChannel endTutorial;

    [Header("BossHealth")]
    [SerializeField] private GameObject bossHealthbarObject;
    [SerializeField] private Image bossHealthbar;

    [Header("Shop")]
    [SerializeField] private GameObject shop;

    private float timer;

    private void Awake()
    {
        controls = Keybindinputmanager.Controls;
    }
    private void OnEnable()
    {
        disableBlackScreen.OnEventRaised += BlackScreenDisable;
        standUp.OnEventRaised += IntroStandUp;
        endTutorial.OnEventRaised += TutorialDone;
    }
    private void OnDisable()
    {
        disableBlackScreen.OnEventRaised -= BlackScreenDisable;
        standUp.OnEventRaised -= IntroStandUp;
        endTutorial.OnEventRaised -= TutorialDone;
    }
    private void Start()
    {
        StartCoroutine(InteractionFieldDisable());
        if (PlayerPrefs.GetInt("NewGame") == 0 && GameManager.Instance.CheckForNewGame)
        {
            StartIntro();
        }
    }
    IEnumerator InteractionFieldDisable()
    {
        yield return null;
        interactionField.SetActive(false);
        interactionField.transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);
    }
    private void Update()
    {
        if (controls.Menu.DebugMenu.WasPerformedThisFrame())
        {
            if (DebugMenu.activeSelf == false)
            {
                GameManager.Instance.ActivateCursor();
                DebugMenu.SetActive(true);
            }
            else
            {
                GameManager.Instance.DeactivateCursor();
                DebugMenu.SetActive(false);
            }
        }
    }
    public void HandleInteractionBox(bool state)
    {
        if(interactionField != null) interactionField.SetActive(state);
    }
    public void InteractionTextUpdate(string text)
    {
        interactionText.text = text + " (<color=green>" + controls.Player.Interact.GetBindingDisplayString() + "</color>)";
    }
    public void HealthUIUpdate(int current, int max)
    {
        healthbar.fillAmount = (float)current / max;
        healthText.text = current + "/" + max;
    }
    public void EnergyUIUpdate(int current, int max)
    {
        energybar.fillAmount = (float)current / max;
        energyText.text = current + "/" + max;
    }
    public void ToggleBossHealth(bool activate)
    {
        bossHealthbarObject.SetActive(activate);
    }
    public void BossHealthUIUpdate(int current, int max)
    {
        bossHealthbar.fillAmount = (float)current / max;
    }
    public void PlayerCurrencyUpdate(int amount)
    {
        GameManager.Instance.playerCurrency += amount;
        currencyText.text = GameManager.Instance.playerCurrency.ToString();

        PlayerPrefs.SetInt("PlayerCurrency", GameManager.Instance.playerCurrency);
    }
    public void MessageBoxEnable(string text)
    {
        GameManager.Instance.ActivateCursor();

        Time.timeScale = 0;
        GameManager.Instance.menuController.gameIsPaused = true;

        messageBox.SetActive(true);
        messageBoxText.text = text;
        messageBoxCloseText.text = "Close (<color=green>" + controls.Player.Interact.GetBindingDisplayString() + "</color>)"; 
    }
    public void MessageBoxDisable()
    {
        GameManager.Instance.DeactivateCursor();

        Time.timeScale = 1;
        GameManager.Instance.menuController.gameIsPaused = false;
        messageBox.SetActive(false);
    }
    public void StartIntro()
    {
        blackScreen.gameObject.SetActive(true);
        dialogBox.GetComponent<DialogBox>().DialogStart(introDialog, false);
        dialogBox.SetActive(true);
        Player.Instance.state = Player.States.Emtpy;
        Player.Instance.ChangeAnimationState("Sleep");
    }
    public void BlackScreenDisable()
    {
        blackScreenColor = blackScreen.color;
        blackScreenColor.a = 1;
        StartCoroutine(FadeBlackScreen());
    }
    IEnumerator FadeBlackScreen()
    {
        float fadeTime = 2;
        timer = fadeTime;
        while (blackScreenColor.a > 0.01f)
        {
            timer -= Time.deltaTime;
            float time = timer / fadeTime;
            blackScreenColor.a = time;
            blackScreen.color = blackScreenColor;
            yield return null;

        }
        blackScreen.gameObject.SetActive(false);
    }
    public void IntroStandUp()
    {
        Player.Instance.ChangeAnimationState("StandUp");
    }
    public void TutorialDone()
    {
        int progression = PlayerPrefs.GetInt("TutorialProgress");
        progression++;
        PlayerPrefs.SetInt("TutorialProgress", progression);
        PlayerPrefs.SetInt("NewGame", 1);
    }
    public void ActivateShop()
    {
        GameManager.Instance.ActivateCursor();

        Time.timeScale = 0;
        GameManager.Instance.menuController.gameIsPaused = true;
        shop.SetActive(true);
    }
    public void DeactivateShop()
    {
        GameManager.Instance.DeactivateCursor();

        GameManager.Instance.menuController.EndPause();
        shop.SetActive(false);
    }
}
