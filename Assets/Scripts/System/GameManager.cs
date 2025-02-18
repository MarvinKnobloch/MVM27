using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public MenuController menuController;
    public PlayerUI playerUI;

    public CheckPoint currentCheckpoint;
    [SerializeField] private bool LoadFormCheckpoint;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else Destroy(gameObject);
    }
    private void Start()
    {
        if (LoadFormCheckpoint)
        {
            float XSpawn = PlayerPrefs.GetFloat("PlayerXSpawn");
            float YSpawn = PlayerPrefs.GetFloat("PlayerYSpawn");
            Vector3 spawn = new Vector3(XSpawn, YSpawn, 0);

            Player.Instance.transform.position = spawn;
        }
    }
}
