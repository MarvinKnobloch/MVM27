using UnityEngine;
using UnityEngine.SceneManagement;
using System;

public class CheckPoint : MonoBehaviour, IInteractables
{
    [SerializeField] private GameObject spawnPosition;
    [SerializeField] private GameObject checkpointImage;
    [NonSerialized] public Collider2D checkpointCollider;

    public GameObject interactObj { get => gameObject; }

    [SerializeField] private string actionText;

    public string interactiontext => actionText;
    private void Awake()
    {
        checkpointCollider = GetComponent<Collider2D>();
    }
    public void Interaction()
    {
        CheckpointInteraction();
    }
    private void CheckpointInteraction()
    {
        if (GameManager.Instance.currentCheckpoint != null)
        {
            GameManager.Instance.currentCheckpoint.checkpointImage.SetActive(false);
            GameManager.Instance.currentCheckpoint.checkpointCollider.enabled = true;
        }

        PlayerPrefs.SetFloat("PlayerXSpawn", spawnPosition.transform.position.x);
        PlayerPrefs.SetFloat("PlayerYSpawn", spawnPosition.transform.position.y + 0.5f);
        PlayerPrefs.SetInt("CurrentLevel", SceneManager.GetActiveScene().buildIndex);

        checkpointImage.SetActive(true);
        Player.Instance.playerInteraction.RemoveInteraction(this);
        checkpointCollider.enabled = false;

        GameManager.Instance.currentCheckpoint = this;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (GameManager.Instance.CheckpointOnSpawn == false)
            {
                GameManager.Instance.CheckpointOnSpawn = true;
                CheckpointInteraction();
            }
            else
            {
                Player.Instance.playerInteraction.AddInteraction(this);
            }
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Player.Instance.playerInteraction.RemoveInteraction(this);
        }
    }


    //private void OnTriggerEnter2D(Collider2D collision)
    //{
    //    if (collision.CompareTag("Player"))
    //    {
    //        CheckpointInteraction();
    //    }
    //}
}
