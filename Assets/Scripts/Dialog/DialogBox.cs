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

    private DialogObj currentDialog;
    private int currentDialogNumber;

    private bool readInput;

    private void Awake()
    {
        controls = Keybindinputmanager.Controls;
    }
    private void Update()
    {
        if (readInput == false)
        {
            readInput = true;
        }
        else
        {
            if (controls.Player.Interact.WasPerformedThisFrame() || controls.Menu.MenuEsc.WasPerformedThisFrame())
            {
                DialogContinue();
            }
        }
    }
    public void DialogStart(DialogObj dialog)
    {
        readInput = false;
        Time.timeScale = 0;
        GameManager.Instance.menuController.gameIsPaused = true;

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
        if (currentDialog.dialogs[currentDialogNumber].characterSprite != null) characterImage.sprite = currentDialog.dialogs[currentDialogNumber].characterSprite;
        else characterImage.sprite = null;

        characterName.text = currentDialog.dialogs[currentDialogNumber].characterName;
        boxText.text = currentDialog.dialogs[currentDialogNumber].dialogText;
    }
    public void DialogBoxDisable()
    {
        readInput = false;
        Time.timeScale = 1;
        GameManager.Instance.menuController.gameIsPaused = false;
        gameObject.SetActive(false);
    }
}
