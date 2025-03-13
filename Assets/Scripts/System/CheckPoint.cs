using UnityEngine;
using UnityEngine.SceneManagement;
using System;

public class CheckPoint : MonoBehaviour, IInteractables
{
    [SerializeField] private GameObject spawnPosition;
    [SerializeField] private float YSpawnOffset;
    [SerializeField] private GameObject checkpointOff;
    [SerializeField] private GameObject checkpointOn;
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
            GameManager.Instance.currentCheckpoint.DeactivateCheckpoint();
        }

        PlayerPrefs.SetFloat("PlayerXSpawn", spawnPosition.transform.position.x);
        PlayerPrefs.SetFloat("PlayerYSpawn", spawnPosition.transform.position.y + YSpawnOffset);
        PlayerPrefs.SetInt("CurrentLevel", SceneManager.GetActiveScene().buildIndex);

        checkpointOff.SetActive(false);
        checkpointOn.SetActive(true);
        Player.Instance.playerInteraction.RemoveInteraction(this);
        checkpointCollider.enabled = false;

        GameManager.Instance.currentCheckpoint = this;
    }
    public void DeactivateCheckpoint()
    {
        GameManager.Instance.currentCheckpoint.checkpointCollider.enabled = true;

        checkpointOff.SetActive(true);
        checkpointOn.SetActive(false);
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
