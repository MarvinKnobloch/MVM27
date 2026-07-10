using UnityEngine;
using System.Collections.Generic;
using AK.Wwise;

public class AudioBankLoader : MonoBehaviour
{
    [SerializeField] private List<Bank> soundBanks;

    private void Awake()
    {
        foreach (var bank in soundBanks)
        {
            bank.LoadAsync();
        }

    }

    private void OnDestroy()
    {
        foreach (var bank in soundBanks)
            {
            bank.Unload();
        }
    }
}