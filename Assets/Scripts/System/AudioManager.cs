using System;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource soundSource;

    [SerializeField] private AudioFiles[] musicFiles;
    [SerializeField] private AudioFiles[] soundFiles;

    public enum MusicSongs
    {
        Empty,
        Song1,
        Song2,
    }
    public enum Sounds
    {
        Empty,
        menuButton,
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
    }

    public void PlaySoundOneshot(int soundClip)
    {
        soundSource.PlayOneShot(soundFiles[soundClip].audioClip, soundFiles[soundClip].volume);
    }
}
[Serializable]
public struct AudioFiles
{
    public AudioClip audioClip;
    public float volume;
}
