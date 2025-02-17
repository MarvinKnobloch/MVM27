using TMPro;
using UnityEngine;

public class InteractionField : MonoBehaviour
{
    private void Start()
    {
        gameObject.SetActive(false);
    }
    private void LateUpdate()
    {
        transform.position = Player.Instance.transform.position + transform.up * 1f;
    }
}
