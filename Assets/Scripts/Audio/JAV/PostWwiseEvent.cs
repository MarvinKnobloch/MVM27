using UnityEngine;
using System.Collections.Generic;

public class PostWwiseEvent : MonoBehaviour
{
    #region Old System

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
    #endregion

    // Types of Lists (with key + events)

    [System.Serializable]
    public struct KeyAudioMapping
    {
        public string audioKey;
        public AK.Wwise.Event wwiseEvent;
    }

    // Events, Switches, etc... 

    [Header("=== ANIMATION TRIGGER EVENTS ===")]
    [Tooltip("One-shot events strictly tied to and triggered by Animation Events in the timeline.")]
    public List<KeyAudioMapping> animationAudioEvents;


    [Header("=== STATE & GAMEPLAY ONE-SHOTS ===")]
    [Tooltip("One-shot events triggered via C# script logic (e.g., collisions, damage calculations).")]
    public List<KeyAudioMapping> impactAudioEvents;


    [Header("=== CONTINUOUS / LOOPING EVENTS ===")]
    [Tooltip("Events that loop and require explicit Start/Stop code execution or key tracking.")]
    public List<KeyAudioMapping> continuousLoopingEvents;


    [Header("=== PERSISTENT MUSIC & AMBIENCE ===")]
    [Tooltip("Global music/ambience events managed via States, Switches, or persistent object calls.")]


    [Header("Wwise Switches")]
    public AK.Wwise.Switch nullElementSwitch;
    public AK.Wwise.Switch fireElementSwitch;
    public AK.Wwise.Switch airElementSwitch;

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

    public void PlayImpactEvent(string key)
    {
        foreach (var mapping in impactAudioEvents)
        {
            if (mapping.audioKey == key)
            {
                mapping.wwiseEvent.Post(gameObject);
                return;
            }
        }
        Debug.LogWarning($"Impact key '{key}' not found in Impact Audio Events on {gameObject.name}!");
    }

    public void PlayContinuousEvent(string key)
    {
        foreach (var mapping in continuousLoopingEvents)
        {
            if (mapping.audioKey == key)
            {
                mapping.wwiseEvent.Post(gameObject);
                return;
            }
        }
        Debug.LogWarning($"Continuous '{key}' not found in Impact Audio Events on {gameObject.name}!");
    }

    /* public void AudioPlayHit()
     {
         PlayImpactEvent("PlayHitEvent");
     }

     public void AudioStartHeal()
     {
         PlayContinuousEvent("PlayHealEvent");
     }

     public void AudioStopHeal()
     {
         PlayContinuousEvent("StopHealEvent");
     }*/
}