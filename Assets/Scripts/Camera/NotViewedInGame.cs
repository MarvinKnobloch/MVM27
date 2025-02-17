using UnityEngine;

public class NotViewedInGame : MonoBehaviour
{
    //attch to game objects you only want to see in the editor.
    private void Start()
    {
        GetComponent<SpriteRenderer>().enabled = false;
    }
}
