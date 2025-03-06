using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using TMPro;

public class Audioslider : MonoBehaviour
{
    public AudioMixer audioMixer;
    public string volumeString;
    private Slider slider;

    [SerializeField] private TextMeshProUGUI sliderText;

    private bool skipFirstSound;

    private void Awake()
    {
        slider = GetComponent<Slider>();
        skipFirstSound = true;
    }
    private void OnEnable()
    {

        float soundvalue = PlayerPrefs.GetFloat(volumeString);
        float textvalue;
        if (volumeString == "SoundVolume")
        {
            slider.value = soundvalue + 80;
            textvalue = soundvalue + 80;
        }
        else 
        {
            slider.value = soundvalue + 100;
            textvalue = (soundvalue + 100); 
        }
        sliderText.text = Mathf.Round(textvalue).ToString();
    }
    public void MasterValuechange(float slidervalue)
    {
        float textvalue = slidervalue;
        sliderText.text = Mathf.Round(textvalue).ToString();

        slidervalue -= 100;

        PlayerPrefs.SetInt("AudioHasBeenChange", 1);

        audioMixer.SetFloat(volumeString, slidervalue);
        bool gotvalue = audioMixer.GetFloat(volumeString, out float soundvalue);
        if (gotvalue == true)
        {
            if (soundvalue > 0)
            {
                Debug.Log(soundvalue);
                audioMixer.SetFloat(volumeString, 0);
            }
        }

        PlayerPrefs.SetFloat(volumeString, slidervalue);
    }
    public void MusicValueChange(float slidervalue)
    {
        float textvalue = slidervalue;
        sliderText.text = Mathf.Round(textvalue).ToString();

        slidervalue -= 100;
        PlayerPrefs.SetInt("AudioHasBeenChange", 1);
        
        audioMixer.SetFloat(volumeString, slidervalue);
        bool gotvalue = audioMixer.GetFloat(volumeString, out float soundvalue);
        if (gotvalue == true)
        {
            if (soundvalue > 5)
            {
                Debug.Log(soundvalue);
                audioMixer.SetFloat(volumeString, 0);
            }
        }
        
        PlayerPrefs.SetFloat(volumeString, slidervalue);
    }
    public void SoundEffectValueChange(float slidervalue)
    {
        float textvalue = slidervalue;
        sliderText.text = Mathf.Round(textvalue).ToString();

        SetDecibel(slidervalue, volumeString, 20);
        //float decibel = slidervalue - 80;
        //decibel = Mathf.Log10(decibel) * 20;

        //PlayerPrefs.SetInt("AudioHasBeenChange", 1);

        //audioMixer.SetFloat(volumeString, decibel);
        //bool gotvalue = audioMixer.GetFloat(volumeString, out float soundvalue);
        //if (gotvalue == true)
        //{
        //    if (soundvalue > 20)
        //    {
        //        Debug.Log(soundvalue);
        //        audioMixer.SetFloat(volumeString, 20);
        //    }
        //}

        //PlayerPrefs.SetFloat(volumeString, decibel);

        if(skipFirstSound == true)
        {
            skipFirstSound = false;
            return;
        }
        else AudioManager.Instance.PlaySoundOneshot((int)AudioManager.Sounds.menuButton);
    }
    private void SetDecibel(float sliderValue, string audioString, int maxDecibel)
    {
        PlayerPrefs.SetInt("AudioHasBeenChange", 1);

        float decibel = sliderValue - 80;
        decibel = Mathf.Log10(decibel) * 20;

        audioMixer.SetFloat(audioString, decibel);
        bool gotvalue = audioMixer.GetFloat(audioString, out float soundvalue);

        if (gotvalue == true)
        {
            if (soundvalue > 20)
            {
                Debug.Log(soundvalue);
                audioMixer.SetFloat(audioString, maxDecibel);
            }
        }
        PlayerPrefs.SetFloat(audioString, decibel);

    }
}
