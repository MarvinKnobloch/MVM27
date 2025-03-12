using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource soundSource;
    [SerializeField] private AudioSource footstepSource;

    [Space]
    [SerializeField] private float minPitch;
    [SerializeField] private float maxPitch;
    [SerializeField] private float randomVolumeMultipler;

    [Space]
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private string masterVolume;
    [SerializeField] private string musicVolume;
    [SerializeField] private string soundVolume;

    //Music
    private float disableTimer;
    private DateTime startDate;
    private DateTime currentDate;
    private float seconds;

    [SerializeField] private AudioFiles[] musicFiles;
    [SerializeField] private AudioFiles[] soundFiles;

    [Header("Steps")]
    [SerializeField] public AudioFiles[] nonStepsSounds;
    [SerializeField] public AudioFiles[] fireStepSounds;
    [SerializeField] public AudioFiles[] airStepSounds;

    [Header("Jumps")]
    [SerializeField] public AudioFiles[] nonJumpSounds;
    [SerializeField] public AudioFiles[] fireJumpSounds;
    [SerializeField] public AudioFiles[] airJumpSounds;

    [Header("Dash")]
    [SerializeField] public AudioFiles[] nonDashSounds;
    [SerializeField] public AudioFiles[] fireDashSounds;
    [SerializeField] public AudioFiles[] airDashSounds;

    [Header("Attacks")]
    [SerializeField] public AudioFiles[] nonAttackSounds;
    [SerializeField] public AudioFiles[] fireAttackSounds;
    [SerializeField] public AudioFiles[] airAttackSounds;


    public enum MusicSongs
    {
        Empty,
        Tutorial,
        Boss,
        FireArea,
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
    public void PlayAudioFileOneShot(AudioFiles file)
    {
        soundSource.PlayOneShot(file.audioClip, file.volume);
    }
    public void PlayRandomOneShot(AudioFiles[] files)
    {
        int randomNumber = UnityEngine.Random.Range(0, files.Length);

        soundSource.PlayOneShot(files[randomNumber].audioClip, files[randomNumber].volume);
    }
    public void PlayFootSteps(AudioFiles[] files, int soundClip)
    {
        float randomVolume = files[soundClip].volume * randomVolumeMultipler;
        footstepSource.clip = files[soundClip].audioClip;
        footstepSource.volume = files[soundClip].volume + UnityEngine.Random.Range(-randomVolume, randomVolume);
        footstepSource.pitch = UnityEngine.Random.Range(minPitch, maxPitch);

        footstepSource.Play();
    }

    public void StartMusicFadeOut(int audioFile)
    {
        if (musicSource.clip == musicFiles[audioFile].audioClip) return;
        if(musicFiles[audioFile].audioClip == null) return;

        float fadeOutSpeed;
        if (musicSource.clip == null) fadeOutSpeed = 0.1f;
        else fadeOutSpeed = 4;

        float fadeInSpeed = 4;

        StopAllCoroutines();
        StartCoroutine(FadeOutMusicVolume(audioFile, fadeOutSpeed, fadeInSpeed));
    }
    public IEnumerator FadeOutMusicVolume(int audioFile, float fadeOutSpeed, float fadeInSpeed)
    {
        float duration = fadeOutSpeed;
        float start = musicSource.volume;
        float targetVolume = 0;
        startDate = DateTime.Now;
        disableTimer = 0f;
        while (disableTimer < duration)
        {
            currentDate = DateTime.Now;
            seconds = currentDate.Ticks - startDate.Ticks;
            disableTimer = seconds * 0.0000001f;
            musicSource.volume = Mathf.Lerp(start, targetVolume, disableTimer / duration);
            yield return null;
        }

            musicSource.clip = musicFiles[audioFile].audioClip;
            musicSource.Play();
            StartCoroutine(FadeInMusicVolume(audioFile, fadeInSpeed, 0));
    }
    public IEnumerator FadeInMusicVolume(int audioFile, float fadeinspeed, float startvolume)
    {
        float duration = fadeinspeed;
        float start = startvolume;
        startDate = DateTime.Now;
        disableTimer = 0f;
        while (disableTimer < duration)
        {
            currentDate = DateTime.Now;
            seconds = currentDate.Ticks - startDate.Ticks;
            disableTimer = seconds * 0.0000001f;
            musicSource.volume = Mathf.Lerp(start, musicFiles[audioFile].volume, disableTimer / duration);
            yield return null;
        }
        yield break;
    }
}
[Serializable]
public struct AudioFiles
{
    public AudioClip audioClip;
    public float volume;
}
