using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;
using Unity.VisualScripting;

public class MenuController : MonoBehaviour
{
    private Controls controls;

    private GameObject baseMenu;
    private GameObject currentOpenMenu;
    [NonSerialized] public bool gameIsPaused;

    [SerializeField] private GameObject titleMenu;
    [SerializeField] private GameObject ingameMenu;

    [SerializeField] private GameObject confirmController;
    [SerializeField] private Button confirmButton;
    [SerializeField] private TextMeshProUGUI confirmText;

    private void Awake()
    {
        //controls = Keybindinputmanager.Controls;

        if (SceneManager.GetActiveScene().buildIndex == 0)
        {
            baseMenu = titleMenu;
            baseMenu.SetActive(true);
        }
        else
        {
            baseMenu = ingameMenu;
        }
    }

    void Update()
    {
        if (controls.Menu.MenuEsc.WasPerformedThisFrame())
        {
            HandleMenu();
        }
    }
    private void OnEnable()
    {
        controls.Enable();
    }
    private void OnDisable()
    {
        controls.Disable();
    }
    public void HandleMenu()
    {
        if (SceneManager.GetActiveScene().buildIndex == 0)
        {
            if (titleMenu.activeSelf == true) return;
            else CloseSelectedMenu();
        }
        else
        {
            
            if (Player.Instance == null) return;

            //if (PlayerUI.Instance != null)
            //{
            //    if (PlayerUI.Instance.upgradeController.GetComponent<UpgradeController>().upgrades.activeSelf == true) return; 
            //}

            if (confirmController.activeSelf == true) confirmController.SetActive(false);
            else if (ingameMenu.activeSelf == false)
            {
                if (gameIsPaused == false)
                {
                    PauseGame();
                    ingameMenu.SetActive(true);

                }
                else CloseSelectedMenu();
            }
            else
            {
                ingameMenu.SetActive(false);
                EndPause();
            }
        }
    }

    public void OpenSelection(GameObject currentMenu)
    {
        {
            currentOpenMenu = currentMenu;
            currentMenu.SetActive(true);

            titleMenu.SetActive(false);
            ingameMenu.SetActive(false);

            //AudioController.Instance.PlaySoundOneshot((int)AudioController.Sounds.menuButton);
        }
    }
    //public void SetDifficulty(float percentage)
    //{
    //    hpScaling = percentage;
    //    StartGame();
    //}
    public void StartGame()
    {
        //AudioController.Instance.PlaySoundOneshot((int)AudioController.Sounds.menuButton);
        gameIsPaused = false;
        Time.timeScale = 1;
        SceneManager.LoadScene(1);
    }
    public void ResumeGame()
    {
        ingameMenu.SetActive(false);
        EndPause();
    }
    public void SetRestartConfirm()
    {
        OpenConfirmController(StartGame, "Restart Game?");
    }
    public void SetBackToMainMenuConfirm()
    {
        OpenConfirmController(BackToMainMenu, "Back to main menu?");
    }
    public void BackToMainMenu()
    {
        //AudioController.Instance.PlaySoundOneshot((int)AudioController.Sounds.menuButton);
        gameIsPaused = false;
        Time.timeScale = 1;
        SceneManager.LoadScene(0);
    }
    public void CloseSelectedMenu()
    {
        if (currentOpenMenu != null)
        {
            currentOpenMenu.SetActive(false);
            currentOpenMenu = null; // Clear previous menu after returning
            baseMenu.SetActive(true);
        }
        else
        {
            Debug.LogWarning("No previous menu to return to. Going back to inGameMenu.");
            baseMenu.SetActive(true);
        }
        //AudioController.Instance.PlaySoundOneshot((int)AudioController.Sounds.menuButton);
    }

    private void PauseGame()
    {
        gameIsPaused = true;
        Time.timeScale = 0;

        //AudioController.Instance.PlaySoundOneshot((int)AudioController.Sounds.menuButton);
    }
    private void EndPause()
    {
        //Cursor.lockState = CursorLockMode.Locked;
        //Cursor.visible = false;

        gameIsPaused = false;
        Time.timeScale = 1;

        //AudioController.Instance.PlaySoundOneshot((int)AudioController.Sounds.menuButton);
    }
    private void OpenConfirmController(UnityAction buttonEvent, string text)
    {
        confirmText.text = text;

        confirmButton.onClick.RemoveAllListeners();
        confirmButton.onClick.AddListener(() => buttonEvent());
        confirmController.SetActive(true);
    }
    public void CloseConfirmSelection()
    {
        confirmController.SetActive(false);

        //AudioController.Instance.PlaySoundOneshot((int)AudioController.Sounds.menuButton);
    }
}
