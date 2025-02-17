using UnityEngine;
using TMPro;

public class PlayerUI : MonoBehaviour
{
    [SerializeField] private GameObject interactionField;
    [SerializeField] private TextMeshProUGUI interactionText;

    public void HandleInteractionBox(bool state)
    {
        interactionField.SetActive(state);
    }
    public void InteractionTextUpdate(string text)
    {
        interactionText.text = text;
    }
}
