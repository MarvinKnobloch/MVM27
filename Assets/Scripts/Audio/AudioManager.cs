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
            PlayerPrefs.SetFloat("SliderValue" + masterVolume, 0.8f);
            PlayerPrefs.SetFloat(masterVolume, Mathf.Log10(PlayerPrefs.GetFloat("SliderValue" + masterVolume) * 20));
            SetVolume(masterVolume, 0);

            PlayerPrefs.SetFloat("SliderValue" + musicVolume, 0.8f);
            PlayerPrefs.SetFloat(musicVolume, Mathf.Log10(PlayerPrefs.GetFloat("SliderValue" + musicVolume) * 20));
            SetVolume(musicVolume, 0);

            PlayerPrefs.SetFloat("SliderValue" + soundVolume, 2.4f);
            PlayerPrefs.SetFloat(soundVolume, Mathf.Log10(PlayerPrefs.GetFloat("SliderValue" + soundVolume) * 20));
            SetVolume(soundVolume, 0);
        }
        else
        {
            SetVolume(masterVolume, 0);
            SetVolume(musicVolume, 0);
            SetVolume(soundVolume, 10);
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
