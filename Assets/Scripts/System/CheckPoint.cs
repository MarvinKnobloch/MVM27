using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using System.Collections;

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

    private bool playSound;
    private void Awake()
    {
        checkpointCollider = GetComponent<Collider2D>();
        StartCoroutine(ActivateSound());
    }
    IEnumerator ActivateSound()
    {
        yield return new WaitForSeconds(0.3f);
        playSound = true;
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

        //RestoreHealthEnergy();

        //checkpointCollider.enabled = false;

        GameManager.Instance.currentCheckpoint = this;

        if(playSound) AudioManager.Instance.PlayAudioFileOneShot(AudioManager.Instance.worldSounds[(int)AudioManager.WolrdSounds.CheckPoint]);
    }
    public void DeactivateCheckpoint()
    {
        GameManager.Instance.currentCheckpoint.checkpointCollider.enabled = true;

        checkpointOff.SetActive(true);
        checkpointOn.SetActive(false);
    }

    private void RestoreHealthEnergy()
    {
        int energy = Mathf.RoundToInt(Player.Instance.EnergyMaxValue * 0.5f + PlayerPrefs.GetInt(Upgrades.StatsUpgrades.BonusEnergyRecharge.ToString()));

        if (Player.Instance.health.Value != Player.Instance.health.MaxValue || Player.Instance.EnergyValue < energy)
            AudioManager.Instance.PlayAudioFileOneShot(AudioManager.Instance.utilityFiles[(int)AudioManager.UtilitySounds.MenuAccept]);

        Player.Instance.health.Heal(Player.Instance.health.MaxValue);

        if (Player.Instance.EnergyValue < energy)
        {
            Player.Instance.EnergyValue = energy;
            Player.Instance.EnergyUpdate(0);
        }
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
                if (this != GameManager.Instance.currentCheckpoint)
                {
                    Player.Instance.playerInteraction.AddInteraction(this);
                }

                RestoreHealthEnergy();
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
