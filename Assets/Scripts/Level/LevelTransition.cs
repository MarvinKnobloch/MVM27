using UnityEngine;

public class LevelTransition : MonoBehaviour
{
    [SerializeField] private float xPosition;
    [SerializeField] private float yPosition;
    [SerializeField] private int levelIndex;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerPrefs.SetFloat("PlayerXSpawn", xPosition);
            PlayerPrefs.SetFloat("PlayerYSpawn", yPosition);
            PlayerPrefs.SetInt("CurrentLevel", levelIndex);

            GameManager.Instance.menuController.ResetPlayer(false);
        }
    }
}
