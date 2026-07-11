using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public struct WwiseEventMapping
{
    [Tooltip("The custom key name used to call this sound in code (e.g., 'Player_Jump', 'Sword_Swing')")]
    public string eventKey;

    [Tooltip("The corresponding Wwise Event from your Wwise Project.")]
    public AK.Wwise.Event audioEvent;
}

[System.Serializable]
public struct WwiseSwitchMapping
{
    [Tooltip("The custom key name used to call this switch in code (e.g., 'Grass', 'Stone')")]
    public string switchKey;

    [Tooltip("The corresponding Wwise Switch.")]
    public AK.Wwise.Switch audioSwitch;
}

[DefaultExecutionOrder(-50)]
public class WwiseAudioManager : MonoBehaviour
{
    // === Singleton structure

    public static WwiseAudioManager Instance { get; private set; }

    // === Event categories

    [Header("=== Animation / Sync Events ===")]
    [Tooltip("Audio events strictly driven by Animation Events inside character or enemy timelines.")]
    public List<WwiseEventMapping> animationEvents = new List<WwiseEventMapping>();

    [Header("=== Triggered Fire-and-Forget Events ===")]
    [Tooltip("One-shot occurrences triggered programmatically (e.g., UI clicks, player damage grunts, item pickups).")]
    public List<WwiseEventMapping> triggeredEvents = new List<WwiseEventMapping>();

    [Header("=== Managed / Persistent Events ===")]
    [Tooltip("Continuous events requiring explicit Play and Stop actions (e.g., healing channels, wall-sliding loops).")]
    public List<WwiseEventMapping> managedEvents = new List<WwiseEventMapping>();

    [Header("=== Global Events ===")]
    [Tooltip("Long-running background audio or structural music triggered globally or via zone entry.")]
    public List<WwiseEventMapping> globalEvents = new List<WwiseEventMapping>();

    [Header("=== Global Switches ===")]
    [Tooltip("A modular list of Wwise Switches used to swap audio variations like footstep surfaces.")]
    public List<WwiseSwitchMapping> globalSwitches = new List<WwiseSwitchMapping>();


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

    public void PopulateEventCache(List<WwiseEventMapping> list)
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