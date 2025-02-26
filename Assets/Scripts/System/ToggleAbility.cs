using UnityEngine;
using UnityEngine.UI;

public class ToggleAbility : MonoBehaviour
{
    [SerializeField] GameManager.AbilityStrings abilityString;
    private Image buttonImage;

    private void Awake()
    {
        buttonImage = GetComponent<Image>();
    }
    private void OnEnable()
    {
        if (PlayerPrefs.GetInt(abilityString.ToString()) == 0) buttonImage.color = Color.red;
        else buttonImage.color = Color.green;
    }

    public void Toggle()
    {
        if (PlayerPrefs.GetInt(abilityString.ToString()) == 0)
        {
            PlayerPrefs.SetInt(abilityString.ToString(), 1);
            buttonImage.color = Color.green;
        }
        else
        {
            PlayerPrefs.SetInt(abilityString.ToString(), 0);
            buttonImage.color = Color.red;
        }
        Player.Instance.PlayerAbilityUpdate();
    }
}
