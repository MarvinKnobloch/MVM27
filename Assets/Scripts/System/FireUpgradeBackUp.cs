using UnityEngine;

public class FireUpgradeBackUp : MonoBehaviour
{
    void Start()
    {
        if (PlayerPrefs.GetInt("TutorialBoss") == 0)
        {
            gameObject.SetActive(false);
        }
        else if (PlayerPrefs.GetInt(GameManager.AbilityStrings.FireElement.ToString()) == 1)
        {
            gameObject.SetActive(false);
        }
    }
}
