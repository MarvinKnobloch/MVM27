using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;

public class PlayerUI : MonoBehaviour
{
    [SerializeField] private GameObject interactionField;
    [SerializeField] private TextMeshProUGUI interactionText;
    private Controls controls;

    private void Awake()
    {
        controls = Keybindinputmanager.Controls;
    }
    public void HandleInteractionBox(bool state)
    {
        interactionField.SetActive(state);
    }
    public void InteractionTextUpdate(string text)
    {
        interactionText.text = text + " (<color=green>" + controls.Player.Interact.GetBindingDisplayString() + "</color>)";
    }
}
