using UnityEngine;

public class FireUpgradeBackUp : MonoBehaviour
{
    void Start()
    {
        if (PlayerPrefs.GetInt(GameManager.OverworldSaveNames.TutorialBoss.ToString()) == 0)
        {
            gameObject.SetActive(false);
        }
        else if (PlayerPrefs.GetInt(GameManager.AbilityStrings.FireElement.ToString()) == 1)
        {
            gameObject.SetActive(false);
        }
    }
}
