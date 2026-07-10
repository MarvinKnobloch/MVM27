using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public struct WwiseEventMappingTest
{
    [Tooltip("The custom key name used to call this sound in code (e.g., 'Player_Jump', 'Sword_Swing')")]
    public string eventKey;

    [Tooltip("The corresponding Wwise Event from your Wwise Project.")]
    public AK.Wwise.Event audioEvent;
}

[System.Serializable]

public struct WwiseSwitchMappingTest
{
    [Tooltip("The custom key name used to call this switch in code (e.g., 'Grass', 'Stone')")]
    public string switchKey;

    [Tooltip("The corresponding Wwise Switch.")]
    public AK.Wwise.Switch audioSwitch;
}

[DefaultExecutionOrder(-50)]
public class LearningAudioManager : MonoBehaviour
{
    //Singleton setup
    public static LearningAudioManager Instance { get; private set; }

    //Event Categories
    [Header("=== Animation / Sync Events ===")]
    [Tooltip("Audio events strictly driven by Animation Events inside character or enemy timelines.")]
    [SerializeField] private List<WwiseEventMappingTest> animationEvents = new List<WwiseEventMappingTest>();

    [Header("=== Fire-and-Forget Events ===")]
    [Tooltip("One-shot occurrences triggered programmatically (e.g., UI clicks, player damage grunts, item pickups).")]
    [SerializeField] private List<WwiseEventMappingTest> triggerEvents = new List<WwiseEventMappingTest>();

    [Header("--- Managed / Persistent Events ---")]
    [Tooltip("Continuous events requiring explicit Play and Stop actions (e.g., healing channels, wall-sliding loops).")]
    [SerializeField] private List<WwiseEventMappingTest> managedEvents = new List<WwiseEventMappingTest>();

    [Header("--- Global Events ---")]
    [Tooltip("Long-running background audio or structural music triggered globally or via zone entry.")]
    [SerializeField] private List<WwiseEventMappingTest> globalEvents = new List<WwiseEventMappingTest>();

    [Header("--- Global Switches ---")]
    [Tooltip("A modular list of Wwise Switches used to swap audio variations like footstep surfaces.")]
    [SerializeField] private List<WwiseSwitchMappingTest> globalSwitches = new List<WwiseSwitchMappingTest>();

    //Dictionaries
    private Dictionary<string, AK.Wwise.Event> eventCache = new Dictionary<string, AK.Wwise.Event>();
    private Dictionary<string, AK.Wwise.Switch> switchCache = new Dictionary<string, AK.Wwise.Switch>();

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

    private void InitializeCaches()
    {
        //Populate the event cache
        PopulateEventList(animationEvents);
        PopulateEventList(triggerEvents);
        PopulateEventList(managedEvents);
        PopulateEventList(globalEvents);

        //Populate switches
        foreach (var mapping in globalSwitches)
        {
            if (!string.IsNullOrEmpty(mapping.switchKey) && !switchCache.ContainsKey(mapping.switchKey))
            {
                switchCache.Add(mapping.switchKey, mapping.audioSwitch);
            }
        }
    }

    private void PopulateEventList(List<WwiseEventMappingTest> list)
    {
        foreach (var mapping in list)
        {
            if (string.IsNullOrEmpty(mapping.eventKey)) continue;

            if(!eventCache.ContainsKey(mapping.eventKey))
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
            Debug.LogWarning($"[AudioManager] PlayEvent failed. Key '{eventKey}' not found in cache");
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


