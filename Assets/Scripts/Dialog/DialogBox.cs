using System;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class DialogBox : MonoBehaviour
{
    private Controls controls;

    [SerializeField] private Image characterImage;
    [SerializeField] private TextMeshProUGUI characterName;
    [SerializeField] private TextMeshProUGUI boxText;
    [SerializeField] private GameObject skipButton;

    private DialogObj currentDialog;
    private int currentDialogNumber;

    private bool readInput;

    private bool cantSkipDialog;
    private bool disableInputs;
    private bool pauseGame;
    private float autoPlayInterval;

    private float timer;

    private void Awake()
    {
        controls = Keybindinputmanager.Controls;
    }
    private void Update()
    {
        if (autoPlayInterval != 0)
        {
            timer += Time.deltaTime;
            if(timer > autoPlayInterval)
            {
                timer = 0;
                DialogContinue();
            }
        }
        else
        {
            if (readInput == false)
            {
                readInput = true;
            }
            else
            {
                if (controls.Player.Interact.WasPerformedThisFrame() || controls.Player.Attack.WasPerformedThisFrame())
                {
                    DialogContinue();
                }
                else if (controls.Menu.MenuEsc.WasPerformedThisFrame())
                {
                    if (cantSkipDialog) return;
                    DialogBoxDisable();
                }
            }
        }
    }
    public void DialogStart(DialogObj dialog)
    {
        readInput = false;

        cantSkipDialog = dialog.cantSkipDialog;
        disableInputs = dialog.disableInputs;
        pauseGame = dialog.pauseGame;


        if (cantSkipDialog) skipButton.SetActive(false);
        else skipButton.SetActive(true);

        if (disableInputs) GameManager.Instance.menuController.gameIsPaused = true;
        if (pauseGame) Time.timeScale = 0;

        currentDialog = dialog;
        currentDialogNumber = 0;
        DialogUpdate();
    }
    public void DialogContinue()
    {
        if (currentDialogNumber < currentDialog.dialogs.Length -1)
        {
            currentDialogNumber++;
            DialogUpdate();
        }
        else DialogBoxDisable();
    }
    private void DialogUpdate()
    {
        timer = 0;
        autoPlayInterval = currentDialog.dialogs[currentDialogNumber].autoPlayInterval;

        if (currentDialog.dialogs[currentDialogNumber].characterSprite != null) characterImage.sprite = currentDialog.dialogs[currentDialogNumber].characterSprite;
        else characterImage.sprite = null;

        characterName.text = currentDialog.dialogs[currentDialogNumber].characterName;
        boxText.text = currentDialog.dialogs[currentDialogNumber].dialogText;

        if(currentDialog.dialogs[currentDialogNumber].dialogEvent != null)
        {
            currentDialog.dialogs[currentDialogNumber].dialogEvent.OnEventRaised.Invoke();
        }
    }
    public void DialogBoxDisable()
    {
        if (autoPlayInterval == 0)
        {
            readInput = false;
            Time.timeScale = 1;
        }
        GameManager.Instance.menuController.gameIsPaused = false;
        gameObject.SetActive(false);
    }
}
