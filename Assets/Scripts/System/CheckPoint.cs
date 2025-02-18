using UnityEngine;
using UnityEngine.SceneManagement;

public class CheckPoint : MonoBehaviour
{
    [SerializeField] private GameObject spawnPosition;
    [SerializeField] private GameObject checkpointImage;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if(GameManager.Instance.currentCheckpoint != null)
            {
                GameManager.Instance.currentCheckpoint.checkpointImage.SetActive(false);
            }

            PlayerPrefs.SetFloat("PlayerXSpawn", spawnPosition.transform.position.x);
            PlayerPrefs.SetFloat("PlayerYSpawn", spawnPosition .transform.position.y + 0.5f);
            PlayerPrefs.SetInt("CurrentLevel", SceneManager.GetActiveScene().buildIndex);

            checkpointImage.SetActive(true);
            GameManager.Instance.currentCheckpoint = this;
        }
    }
}
