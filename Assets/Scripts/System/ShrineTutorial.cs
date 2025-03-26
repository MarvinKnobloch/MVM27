using UnityEngine;
using TMPro;
using Unity.VisualScripting;
using UnityEngine.InputSystem;

public class ShrineTutorial : MonoBehaviour
{
    private Controls controls;

    [SerializeField] private VoidEventChannel switchTutorial;
    [SerializeField] private VoidEventChannel dashTutorial;
    [SerializeField] private VoidEventChannel fireballTutorial;
    [SerializeField] private VoidEventChannel wallbreakTutorial;
    private void Awake()
    {
        controls = Keybindinputmanager.Controls;
    }

    private void OnEnable()
    {
        if (switchTutorial != null) switchTutorial.OnEventRaised += SwitchTutorial;
        if (dashTutorial != null) dashTutorial.OnEventRaised += DashTutorial;
        if (fireballTutorial != null) fireballTutorial.OnEventRaised += FireballTutorial;
        if (wallbreakTutorial != null) wallbreakTutorial.OnEventRaised += WallbreakTutorial;
    }
    private void OnDisable()
    {
        if (switchTutorial != null) switchTutorial.OnEventRaised -= SwitchTutorial;
        if (dashTutorial != null) dashTutorial.OnEventRaised -= DashTutorial;
        if (fireballTutorial != null) fireballTutorial.OnEventRaised -= FireballTutorial;
        if (wallbreakTutorial != null) wallbreakTutorial.OnEventRaised -= WallbreakTutorial;
    }

    private void SwitchTutorial()
    {
        TextMeshProUGUI dialogText = GameManager.Instance.playerUI.dialogBox.GetComponent<DialogBox>().boxText;
        dialogText.text = ".. This power... this flame... I understand now....\n\n(Press <color=green>" +
        controls.Player.Element2.GetBindingDisplayString() + "</color> to switch to the Fire Element)\n(Press <color=green>" +
        controls.Player.Element1.GetBindingDisplayString() + "</color> to switch back to the Null Element)";
    }
    private void DashTutorial()
    {
        TextMeshProUGUI dialogText = GameManager.Instance.playerUI.dialogBox.GetComponent<DialogBox>().boxText;
        dialogText.text = "With this flame, I’ll charge ahead... I’ll walk through fire and dash through lava....\n\n(Press <color=green>" +
        controls.Player.Dash.GetBindingDisplayString() + "</color> to dash. You are now immune to fire and can dash through lava walls while in Fire Form)";
    }
    private void FireballTutorial()
    {
        TextMeshProUGUI dialogText = GameManager.Instance.playerUI.dialogBox.GetComponent<DialogBox>().boxText;
        dialogText.text = "If I hold up my hand...\n\n(Press <color=green>" +
            controls.Player.ElementAbility1.GetBindingDisplayString() + "</color> in Fire Form to shoot a fireball which burns down obstacles or toggle switches)";
    }
    private void WallbreakTutorial()
    {
        TextMeshProUGUI dialogText = GameManager.Instance.playerUI.dialogBox.GetComponent<DialogBox>().boxText;
        dialogText.text = "... Like I could break through anything...\n\n(Press <color=green>" +
             controls.Player.ElementAbility2.GetBindingDisplayString() + "</color> in Null Form to destroy cracked walls or damage enemies)";
    }
}
