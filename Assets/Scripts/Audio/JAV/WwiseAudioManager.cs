using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct WwiseEventMapping
{
    [Tooltip("The custom key name used to call this sound in code (e.g., 'Player_Jump', 'Sword_Swing').")]
    public string Key;
    [Tooltip("The corresponding Wwise Event from your Wwise Project.")]
    public AK.Wwise.Event WwiseEvent;
}

[System.Serializable]
public struct WwiseSwitchMapping
{
    [Tooltip("The custom key name used to call this switch in code (e.g., 'Grass', 'Stone').")]
    public string Key;
    [Tooltip("The corresponding Wwise Switch.")]
    public AK.Wwise.Switch WwiseSwitch;
}

[DefaultExecutionOrder(-50)] // Ensures the manager initializes before other scripts attempt to call it
public class WwiseAudioManager : MonoBehaviour
{
    public static WwiseAudioManager Instance { get; private set; }

    [Header("--- Animation / Sync Events ---")]
    [Tooltip("Audio events strictly driven by Animation Events inside character or enemy timelines.")]
    [SerializeField] private List<WwiseEventMapping> animationEvents = new List<WwiseEventMapping>();

    [Header("--- Fire-and-Forget Events ---")]
    [Tooltip("One-shot occurrences triggered programmatically (e.g., UI clicks, player damage grunts, item pickups).")]
    [SerializeField] private List<WwiseEventMapping> triggerEvents = new List<WwiseEventMapping>();

    [Header("--- Managed / Persistent Events ---")]
    [Tooltip("Continuous events requiring explicit Play and Stop actions (e.g., healing channels, wall-sliding loops).")]
    [SerializeField] private List<WwiseEventMapping> managedEvents = new List<WwiseEventMapping>();

    [Header("--- Interactive Music & Ambience ---")]
    [Tooltip("Long-running background audio or structural music triggered globally or via zone entry.")]
    [SerializeField] private List<WwiseEventMapping> globalEvents = new List<WwiseEventMapping>();

    [Header("--- Global Switches ---")]
    [Tooltip("A modular list of Wwise Switches used to swap audio variations like footstep surfaces.")]
    [SerializeField] private List<WwiseSwitchMapping> globalSwitches = new List<WwiseSwitchMapping>();

    // High-performance runtime lookups
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
        // Populate events dynamically from all list categories into a single O(1) Lookup Table
        PopulateEventList(animationEvents);
        PopulateEventList(triggerEvents);
        PopulateEventList(managedEvents);
        PopulateEventList(globalEvents);

        // Populate switches
        foreach (var mapping in globalSwitches)
        {
            if (!string.IsNullOrEmpty(mapping.Key) && !switchCache.ContainsKey(mapping.Key))
            {
                switchCache.Add(mapping.Key, mapping.WwiseSwitch);
            }
        }
    }

    private void PopulateEventList(List<WwiseEventMapping> list)
    {
        foreach (var mapping in list)
        {
            if (string.IsNullOrEmpty(mapping.Key)) continue;

            if (!eventCache.ContainsKey(mapping.Key))
            {
                eventCache.Add(mapping.Key, mapping.WwiseEvent);
            }
            else
            {
                Debug.LogWarning($"[AudioManager] Duplicate event key found: '{mapping.Key}'. Skipping duplicates.");
            }
        }
    }

    /// <summary>
    /// Posts a Wwise event using your custom key string.
    /// </summary>
    /// <param name="key">The custom inspector identifier string.</param>
    /// <param name="sourceEmitter">Optional: Pass the GameObject emitting the sound to preserve 3D positioning.</param>
    public void PlayEvent(string key, GameObject sourceEmitter = null)
    {
        GameObject target = sourceEmitter != null ? sourceEmitter : gameObject;

        if (eventCache.TryGetValue(key, out AK.Wwise.Event wwiseEvent))
        {
            wwiseEvent.Post(target);
        }
        else
        {
            Debug.LogWarning($"[AudioManager] Event key '{key}' not found in registry.");
        }
    }

    /// <summary>
    /// Explicitly stops an event (Crucial for your Looping SFX category).
    /// </summary>
    public void StopEvent(string key, GameObject sourceEmitter = null)
    {
        GameObject target = sourceEmitter != null ? sourceEmitter : gameObject;

        if (eventCache.TryGetValue(key, out AK.Wwise.Event wwiseEvent))
        {
            wwiseEvent.Stop(target);
        }
    }

    /// <summary>
    /// Sets a Wwise switch using your custom key string.
    /// </summary>
    public void SetGlobalSwitch(string key, GameObject sourceEmitter = null)
    {
        GameObject target = sourceEmitter != null ? sourceEmitter : gameObject;

        if (switchCache.TryGetValue(key, out AK.Wwise.Switch wwiseSwitch))
        {
            wwiseSwitch.SetValue(target);
        }
        else
        {
            Debug.LogWarning($"[AudioManager] Switch key '{key}' not found in registry.");
        }
    }
}