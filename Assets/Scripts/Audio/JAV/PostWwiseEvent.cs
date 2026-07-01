using UnityEngine;
using System.Collections.Generic;

public class PostWwiseEvent : MonoBehaviour
{
    /*
    public List<AK.Wwise.Event> WwiseEvents;

    public void PlayEvent()
    {
        foreach (var wwiseEvent in WwiseEvents)
        {
        wwiseEvent.Post(gameObject);
        }
    }
    */

    [System.Serializable]
    public struct AnimationAudioMapping
    {
        public string audioKey;
        public AK.Wwise.Event wwiseEvent;
    }

    // Events, Switches, etc... 

    public List<AnimationAudioMapping> animationAudioEvents;

    [Header("Wwise Switches")]
    public AK.Wwise.Switch nullElementSwitch;
    public AK.Wwise.Switch fireElementSwitch;
    public AK.Wwise.Switch airElementSwitch;

    [Header("Continous Ability Events")]
    public AK.Wwise.Event playHealEvent;
    public AK.Wwise.Event stopHealEvent;

    public void SetElementalForm(int elementNumber)
    {
        switch (elementNumber)
        {
            case 0:
                nullElementSwitch.SetValue(gameObject);
                break;

            case 1:
                fireElementSwitch.SetValue(gameObject);
                break;

            case 2:
                airElementSwitch.SetValue(gameObject);
                break;
        }
    }

    public void PlayTaggedEvent(string key)
    {
        foreach (var mapping in animationAudioEvents)
        {
            if (mapping.audioKey == key)
            {
                mapping.wwiseEvent.Post(gameObject);
                return;
            }
        }
        Debug.LogWarning($"Audio key '{key}' not found on {gameObject.name!}");
    }

    public void AudioStartHeal()
    {
        playHealEvent.Post(gameObject);
    }

    public void AudioStopHeal()
    {
        stopHealEvent.Post(gameObject);
    }
}