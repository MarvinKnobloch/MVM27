using UnityEngine;
using System.Collections.Generic;
using AK.Wwise;

public class AudioBankLoader : MonoBehaviour
{
    // These enums replicate Wwise's native trigger selections
    public enum BankLoadTrigger { Awake, Start, Manual }
    public enum BankUnloadTrigger { Destroy, Manual }

    [System.Serializable]
    public struct BankLoadConfiguration
    {
        [Tooltip("The Wwise SoundBank asset to load.")]
        public Bank bank;

        [Tooltip("When should this specific bank be loaded?")]
        public BankLoadTrigger loadTrigger;

        [Tooltip("When should this specific bank be unloaded?")]
        public BankUnloadTrigger unloadTrigger;
    }

    [Header("SoundBank Configurations")]
    [SerializeField] private List<BankLoadConfiguration> soundBanks = new List<BankLoadConfiguration>();

    private void Awake()
    {
        TriggerBankActions(BankLoadTrigger.Awake);
    }

    private void Start()
    {
        TriggerBankActions(BankLoadTrigger.Start);
    }

    private void OnDestroy()
    {
        TriggerBankUnloadActions(BankUnloadTrigger.Destroy);
    }

    /// <summary>
    /// Evaluates the list and loads banks configured for the matching timeline trigger.
    /// </summary>
    private void TriggerBankActions(BankLoadTrigger triggerToExecute)
    {
        foreach (var config in soundBanks)
        {
            // Skip if the bank slot in the inspector is completely blank
            if (config.bank == null) continue;

            if (config.loadTrigger == triggerToExecute)
            {
                config.bank.LoadAsync();
            }
        }
    }

    /// <summary>
    /// Evaluates the list and unloads banks configured for the matching timeline trigger.
    /// </summary>
    private void TriggerBankUnloadActions(BankUnloadTrigger triggerToExecute)
    {
        foreach (var config in soundBanks)
        {
            if (config.bank == null) continue;

            if (config.unloadTrigger == triggerToExecute)
            {
                config.bank.Unload();
            }
        }
    }

    // === OPTIONAL: Public manual bypass triggers if you ever choose "Manual" in the inspector ===

    public void ManuallyLoadBank(string bankName)
    {
        foreach (var config in soundBanks)
        {
            // Note: Replace '.Name' with whatever property matches your version of AK.Wwise.Bank if different
            if (config.bank != null && config.bank.Name == bankName)
            {
                config.bank.LoadAsync();
                return;
            }
        }
    }

    public void ManuallyUnloadBank(string bankName)
    {
        foreach (var config in soundBanks)
        {
            if (config.bank != null && config.bank.Name == bankName)
            {
                config.bank.Unload();
                return;
            }
        }
    }
}