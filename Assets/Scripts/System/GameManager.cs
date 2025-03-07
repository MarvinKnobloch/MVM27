using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public MenuController menuController;
    public PlayerUI playerUI;

    public CheckPoint currentCheckpoint;
    public bool LoadFormCheckpoint;
    public bool CheckForNewGame;
    [NonSerialized] public bool CheckpointOnSpawn;

    [NonSerialized] public int playerCurrency;
    public enum AbilityStrings
    {
        FireElement,
        Fireball,
        WallBreak,
        AirElement,
        PlayerDoubleJump,
        WallBoost,
        PlayerDash,
    }
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else Destroy(gameObject);

        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 120;

        PlayerPrefs.SetInt("NewGame", 1);
    }
    private void Start()
    {
        if (Player.Instance == null) return;

        if (LoadFormCheckpoint)
        {
            float XSpawn = PlayerPrefs.GetFloat("PlayerXSpawn");
            float YSpawn = PlayerPrefs.GetFloat("PlayerYSpawn");
            Vector3 spawn = new Vector3(XSpawn, YSpawn, 0);

            Player.Instance.transform.position = spawn;
            StartCoroutine(CheckPointOnLoad());

        }
        playerUI.PlayerCurrencyUpdate(PlayerPrefs.GetInt("PlayerCurrency"));

        PlayerPrefs.SetInt("CurrentLevel", SceneManager.GetActiveScene().buildIndex);
    }
    IEnumerator CheckPointOnLoad()
    {
        yield return new WaitForSeconds(0.3f);
        CheckpointOnSpawn = true;
    }

    public void ActivateCursor()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
    public void DeactivateCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}
