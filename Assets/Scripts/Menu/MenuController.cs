using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;

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

    [SerializeField] private GameObject loadGameButton;

    private void Awake()
    {
        controls = Keybindinputmanager.Controls;
    }
    private void Start()
    {
        if (SceneManager.GetActiveScene().buildIndex == 0)
        {
            baseMenu = titleMenu;
            baseMenu.SetActive(true);
            GameManager.Instance.playerUI.gameObject.SetActive(false);
            if(PlayerPrefs.GetInt("NewGame") == 0)
            {
                loadGameButton.GetComponent<Button>().enabled = false;
                loadGameButton.transform.GetChild(1).gameObject.SetActive(true);
            }
            else
            {
                loadGameButton.GetComponent<Button>().enabled = true;
                loadGameButton.transform.GetChild(1).gameObject.SetActive(false);
            }
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
            if (confirmController.activeSelf == true) confirmController.SetActive(false);
            else if (titleMenu.activeSelf == true) return;
            else CloseSelectedMenu();
        }
        else
        {
            
            if (Player.Instance == null) return;

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

            AudioManager.Instance.PlaySoundOneshot((int)AudioManager.Sounds.menuButton);
        }
    }

    public void ResumeGame()
    {
        ingameMenu.SetActive(false);
        EndPause();
    }
    public void SetNewGame()
    {
        OpenConfirmController(NewGame, "Start new game?");
    }
    public void SetBackToMainMenuConfirm()
    {
        OpenConfirmController(BackToMainMenu, "Back to main menu?");
    }
    public void NewGame()
    {
        PlayerPrefs.SetInt("NewGame", 1);

        PlayerPrefs.SetFloat("PlayerXSpawn", 1);
        PlayerPrefs.SetFloat("PlayerYSpawn", 1);
        PlayerPrefs.SetInt("CurrentLevel", 1);

        //Abilities
        //Heal???
        //Dash???
        PlayerPrefs.SetInt("FireElement", 0);
        PlayerPrefs.SetInt("Fireball", 0);
        PlayerPrefs.SetInt("WallBreak", 0);
        PlayerPrefs.SetInt("AirElement", 0);
        PlayerPrefs.SetInt("PlayerDoubleJump", 0);
        PlayerPrefs.SetInt("WallBoost", 0);

        //Values
        PlayerPrefs.SetInt("PlayerGold", 0);
        PlayerPrefs.SetInt("BonusHealth", 0);
        PlayerPrefs.SetInt("BonusHeal", 0);
        PlayerPrefs.SetInt("BonusEnergy", 0);
        PlayerPrefs.SetInt("BonusEnergyRecharge", 0);
        PlayerPrefs.SetInt("BonusAttack", 0);
        PlayerPrefs.SetInt("BonusComboEndDamage", 0);
        PlayerPrefs.SetInt("BonusSwitchDamage", 0);
        //FireBallDamage???

        AudioManager.Instance.PlaySoundOneshot((int)AudioManager.Sounds.menuButton);
        gameIsPaused = false;
        Time.timeScale = 1;
        SceneManager.LoadScene(PlayerPrefs.GetInt("CurrentLevel"));
    }
    public void LoadGame()
    {
        AudioManager.Instance.PlaySoundOneshot((int)AudioManager.Sounds.menuButton);
        gameIsPaused = false;
        Time.timeScale = 1;
        SceneManager.LoadScene(PlayerPrefs.GetInt("CurrentLevel"));
    }
    public void ResetPlayer()
    {
        AudioManager.Instance.PlaySoundOneshot((int)AudioManager.Sounds.menuButton);
        gameIsPaused = false;
        Time.timeScale = 1;
        SceneManager.LoadScene(PlayerPrefs.GetInt("CurrentLevel"));
    }
    public void BackToMainMenu()
    {
        AudioManager.Instance.PlaySoundOneshot((int)AudioManager.Sounds.menuButton);
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
        AudioManager.Instance.PlaySoundOneshot((int)AudioManager.Sounds.menuButton);
    }

    private void PauseGame()
    {
        gameIsPaused = true;
        Time.timeScale = 0;

        AudioManager.Instance.PlaySoundOneshot((int)AudioManager.Sounds.menuButton);
    }
    private void EndPause()
    {
        //Cursor.lockState = CursorLockMode.Locked;
        //Cursor.visible = false;

        gameIsPaused = false;
        Time.timeScale = 1;

        AudioManager.Instance.PlaySoundOneshot((int)AudioManager.Sounds.menuButton);
    }
    private void OpenConfirmController(UnityAction buttonEvent, string text)
    {

        confirmText.text = text;

        confirmButton.onClick.RemoveAllListeners();
        confirmButton.onClick.AddListener(() => buttonEvent());
        confirmController.SetActive(true);

        AudioManager.Instance.PlaySoundOneshot((int)AudioManager.Sounds.menuButton);
    }
    public void CloseConfirmSelection()
    {
        confirmController.SetActive(false);

        AudioManager.Instance.PlaySoundOneshot((int)AudioManager.Sounds.menuButton);
    }
}
