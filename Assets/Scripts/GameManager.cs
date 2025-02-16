using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public MenuController menuController;
    public PlayerUI playerUI;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else Destroy(gameObject);
    }
}
