using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using System;

public class PlayerUI : MonoBehaviour
{
    private Controls controls;
    [SerializeField] private GameObject DebugMenu;

    [Header("Interaction")]
    [SerializeField] private GameObject interactionField;
    [SerializeField] private TextMeshProUGUI interactionText;

    [Header("Health")]
    [SerializeField] private Image healthbar;
    [SerializeField] private TextMeshProUGUI healthText;

    [Header("Energy")]
    [SerializeField] private Image energybar;
    [SerializeField] private TextMeshProUGUI energyText;

    [Header("Gold")]
    [SerializeField] private TextMeshProUGUI goldText;

    private void Awake()
    {
        controls = Keybindinputmanager.Controls;
    }
    private void Update()
    {
        if (controls.Menu.DebugMenu.WasPerformedThisFrame())
        {
            if (DebugMenu.activeSelf == false) DebugMenu.SetActive(true);
            else DebugMenu.SetActive(false);
        }
    }
    public void HandleInteractionBox(bool state)
    {
        if(interactionField != null) interactionField.SetActive(state);
    }
    public void InteractionTextUpdate(string text)
    {
        interactionText.text = text + " (<color=green>" + controls.Player.Interact.GetBindingDisplayString() + "</color>)";
    }
    public void HealthUIUpdate(int current, int max)
    {
        healthbar.fillAmount = (float)current / max;
        healthText.text = current + "/" + max;
    }
    public void EnergyUIUpdate(int current, int max)
    {
        energybar.fillAmount = (float)current / max;
        energyText.text = current + "/" + max;
    }
    public void GoldUpdate(int amount)
    {
        GameManager.Instance.playerGold += amount;
        goldText.text = GameManager.Instance.playerGold.ToString();

        PlayerPrefs.SetInt("PlayerGold", GameManager.Instance.playerGold);
    }
}
