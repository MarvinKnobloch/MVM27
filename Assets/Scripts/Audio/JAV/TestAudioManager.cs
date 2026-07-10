using UnityEngine;
using System.Collections.Generic;

// Structs for the events and switches lists

[System.Serializable]
public struct LearningWwiseEventMapping
{
    public string eventKey;
    public AK.Wwise.Event audioEvent;
}

[System.Serializable]
public struct LearningWwiseSwitchMapping
{
    public string switchKey;
    public AK.Wwise.Switch audioSwitch;
}

[DefaultExecutionOrder(-50)]
public class TestAudioManager : MonoBehaviour
{
    //Singleton set up
    public static TestAudioManager Instance { get; private set; }

    #region Inspector Lists
    [Header("=== Animation / Synced Events===")]
    public List<LearningWwiseEventMapping> animationEvents = new List<LearningWwiseEventMapping>();

    [Header("=== Fire-and-Forget Events ===")]
    public List<LearningWwiseEventMapping> triggeredEvents = new List<LearningWwiseEventMapping>();

    [Header("=== Continuous / Looping Events ===")]
    public List<LearningWwiseEventMapping> managedEvents = new List<LearningWwiseEventMapping>();

    [Header("=== Long-running global Events ===")]
    public List<LearningWwiseEventMapping> globalEvents = new List<LearningWwiseEventMapping>();

    [Header("=== Wwise Switches ===")]
    public List<LearningWwiseSwitchMapping> globalSwitches = new List<LearningWwiseSwitchMapping>();
    #endregion

    //Event and Switch Caches
    Dictionary<string, AK.Wwise.Event> eventCache = new Dictionary<string, AK.Wwise.Event>();
    Dictionary<string, AK.Wwise.Switch> switchCache = new Dictionary<string, AK.Wwise.Switch>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        //InitializeCaches();
    }

   // public void InitializeCaches();
    //{
        //PopulateEventList 
   //}

    public void PopulateEventList(List<LearningWwiseEventMapping> list)

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
                Debug.LogWarning($"[AudioManager] PlayEvent failed. Key '{mapping.eventKey}'not found in cache");
            }
                       
        }
    }
}
