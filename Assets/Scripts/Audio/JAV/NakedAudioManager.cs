using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public struct NakedWwiseEventMapping
{
    public string eventKey;
    public AK.Wwise.Event audioEvent;
}

[System.Serializable]
public struct NakedWwiseSwitchMapping
{
    public string switchKey;
    public AK.Wwise.Switch audioSwitch;
}

[DefaultExecutionOrder(-50)]
public class NakedAudioManager : MonoBehaviour
{
    // === Singleton structure

    public static NakedAudioManager Instance { get; private set; }

    // === Event categories

    public List<NakedWwiseEventMapping> animationEvents = new List<NakedWwiseEventMapping>();

    public List<NakedWwiseEventMapping> triggeredEvents = new List<NakedWwiseEventMapping>();

    public List<NakedWwiseEventMapping> managedEvents = new List<NakedWwiseEventMapping>();

    public List<NakedWwiseEventMapping> globalEvents = new List<NakedWwiseEventMapping>();

    public List<NakedWwiseSwitchMapping> globalSwitches = new List<NakedWwiseSwitchMapping>();


    // === Dictionaries

    public Dictionary<string, AK.Wwise.Event> eventCache = new Dictionary<string, AK.Wwise.Event>();
    public Dictionary<string, AK.Wwise.Switch> switchCache = new Dictionary<string, AK.Wwise.Switch>();

    // === Methods

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        DontDestroyOnLoad(gameObject);

        InitializeCaches();
    }

    public void InitializeCaches()
    {
        //Populate Events Cache
        PopulateEventCache(animationEvents);
        PopulateEventCache(triggeredEvents);
        PopulateEventCache(managedEvents);
        PopulateEventCache(globalEvents);

        //Populate Switches Cache
        foreach (var mapping in globalSwitches)
        {
            if (string.IsNullOrEmpty(mapping.switchKey)) continue;

            if (!switchCache.ContainsKey(mapping.switchKey))
            {
                switchCache.Add(mapping.switchKey, mapping.audioSwitch);
            }
            else
            {
                Debug.LogWarning($"[AudioManager] Duplicate switch key found: '{mapping.switchKey}'. Skipping duplicates.");
            }
            
        }
    }

    public void PopulateEventCache(List<NakedWwiseEventMapping> list)
    {
        foreach (var mapping in list)
        {
            if (string.IsNullOrEmpty(mapping.eventKey)) continue;
            
            if (!eventCache.ContainsKey(mapping.eventKey))
            {
                eventCache.Add(mapping.eventKey, mapping.audioEvent);
            }
            else
            {
                Debug.LogWarning($"[AudioManager] Duplicate event key found: '{mapping.eventKey}'. Skipping duplicates.");
            }         
        }
    }

    public void TriggerEvent(string eventKey, GameObject target)
    {
        if (string.IsNullOrEmpty(eventKey)) return;

        if (eventCache.TryGetValue(eventKey, out AK.Wwise.Event audioEvent))
        {
            audioEvent.Post(target);
        }
        else
        {
            Debug.LogWarning($"[AudioManager] Trigger Event failed. Key '{eventKey}' not found in cache.");
        }
    }

    public void SetSwitch(string switchKey, GameObject target)
    {
        if (string.IsNullOrEmpty(switchKey)) return;

        if (switchCache.TryGetValue(switchKey, out AK.Wwise.Switch audioSwitch))
        {
            audioSwitch.SetValue(target);
        }
        else
        {
            Debug.LogWarning($"[AudioManager] SetSwitch failed. Key '{switchKey}' not found in cache");
        }
    }
}