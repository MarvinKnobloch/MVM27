using UnityEngine;

public class DialogTriggerZone : MonoBehaviour
{
    [SerializeField] private DialogObj dialog;
    public void Interaction()
    {
        GameManager.Instance.playerUI.dialogBox.GetComponent<DialogBox>().DialogStart(dialog);
        GameManager.Instance.playerUI.dialogBox.SetActive(true);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            GameManager.Instance.playerUI.dialogBox.GetComponent<DialogBox>().DialogStart(dialog);
            GameManager.Instance.playerUI.dialogBox.SetActive(true);
            gameObject.SetActive(false);
        }
    }
}
