using System;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource soundSource;

    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private string masterVolume;
    [SerializeField] private string musicVolume;
    [SerializeField] private string soundVolume;

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
    private void Start()
    {
        if (PlayerPrefs.GetInt("AudioHasBeenChange") == 0)
        {
            PlayerPrefs.SetFloat(masterVolume, -10);
            audioMixer.SetFloat(masterVolume, PlayerPrefs.GetFloat(masterVolume));
            PlayerPrefs.SetFloat(musicVolume, -8);
            audioMixer.SetFloat(musicVolume, PlayerPrefs.GetFloat(musicVolume));
            PlayerPrefs.SetFloat(soundVolume, 10);
            audioMixer.SetFloat(soundVolume, PlayerPrefs.GetFloat(soundVolume));
        }
        else
        {
            SetVolume(masterVolume, 0);
            SetVolume(musicVolume, 0);
            SetVolume(soundVolume, 20);
        }
    }
    private void SetVolume(string volumename, float maxdb)
    {
        audioMixer.SetFloat(volumename, PlayerPrefs.GetFloat(volumename));
        bool gotvalue = audioMixer.GetFloat(volumename, out float soundvalue);
        if (gotvalue == true)
        {
            if (soundvalue > maxdb)
            {
                audioMixer.SetFloat(volumename, maxdb);
            }
        }
    }
    public void SetSong(int songNumber)
    {
        if (musicSource.clip == musicFiles[songNumber].audioClip) return;

        musicSource.volume = musicFiles[songNumber].volume;
        musicSource.clip = musicFiles[songNumber].audioClip;
        musicSource.Play();
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
