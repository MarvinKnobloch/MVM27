using UnityEngine;

public class HiddenArea : MonoBehaviour
{
    [SerializeField] private GameObject areaToDisable;
    [SerializeField] private GameManager.OverworldSaveNames saveName;

    private void Start()
    {
        if (saveName != GameManager.OverworldSaveNames.Empty)
        {
            if (GameManager.Instance.LoadProgress(saveName) == true)
            {
                Deactiavte();
            }
        }
    }
    private void Deactiavte()
    {
        areaToDisable.SetActive(false);
        gameObject.SetActive(false);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Deactiavte();
        }
    }
}
