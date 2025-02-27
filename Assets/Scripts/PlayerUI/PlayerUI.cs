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

    [Header("Gold")]
    [SerializeField] private TextMeshProUGUI goldText;

    [Header("MessageBox")]
    public GameObject messageBox;
    [SerializeField] private TextMeshProUGUI messageBoxText;

    [Header("DialogBox")]
    public GameObject dialogBox;

    [Header("Intro")]
    [SerializeField] private DialogObj introDialog;
    [SerializeField] private VoidEventChannel disableBlackScreen;
    [SerializeField] private VoidEventChannel standUp;
    [SerializeField] private VoidEventChannel endIntro;

    private float timer;

    private void Awake()
    {
        controls = Keybindinputmanager.Controls;
    }
    private void OnEnable()
    {
        disableBlackScreen.OnEventRaised += BlackScreenDisable;
        standUp.OnEventRaised += IntroStandUp;
        endIntro.OnEventRaised += IntroDone;
    }
    private void OnDisable()
    {
        disableBlackScreen.OnEventRaised -= BlackScreenDisable;
        standUp.OnEventRaised -= IntroStandUp;
        endIntro.OnEventRaised -= IntroDone;
    }
    private void Start()
    {
        StartCoroutine(InteractionFieldDisable());
        if (PlayerPrefs.GetInt("NewGame") == 0)
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
            if (DebugMenu.activeSelf == false) DebugMenu.SetActive(true);
            else DebugMenu.SetActive(false);
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
    public void GoldUpdate(int amount)
    {
        GameManager.Instance.playerGold += amount;
        goldText.text = GameManager.Instance.playerGold.ToString();

        PlayerPrefs.SetInt("PlayerGold", GameManager.Instance.playerGold);
    }
    public void MessageBoxEnable(string text)
    {
        Time.timeScale = 0;
        GameManager.Instance.menuController.gameIsPaused = true;

        messageBox.SetActive(true);
        messageBoxText.text = text;
    }
    public void MessageBoxDisable()
    {
        Time.timeScale = 1;
        GameManager.Instance.menuController.gameIsPaused = false;
        messageBox.SetActive(false);
    }
    public void StartIntro()
    {
        blackScreen.gameObject.SetActive(true);
        dialogBox.GetComponent<DialogBox>().DialogStart(introDialog);
        dialogBox.SetActive(true);
        Player.Instance.state = Player.States.Emtpy;
        Player.Instance.ChangeAnimationState("PlayerSleep");
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
        Player.Instance.ChangeAnimationState("PlayerStandUp");
    }
    public void IntroDone()
    {
        PlayerPrefs.SetInt("NewGame", 1);
    }
}
