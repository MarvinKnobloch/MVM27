using System.Collections.Generic;
using UnityEngine;

public class PressurePlate : MonoBehaviour
{
    [SerializeField] private LayerMask triggerLayer;
    private List<GameObject> objsOnPlatform = new List<GameObject>();
    [SerializeField] private GameObject[] objsToControl;

    private Transform spriteTransform;
    private float spriteMoveOffset = 0.2f;
    private void Awake()
    {
        spriteTransform = transform.GetChild(0).transform;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (Utility.LayerCheck(collision, triggerLayer))
        {
            if(objsOnPlatform.Contains(collision.gameObject) == false)
            {
                objsOnPlatform.Add(collision.gameObject);
                if(objsOnPlatform.Count == 1)
                {
                    foreach (GameObject obj in objsToControl)
                    {
                        if (obj.TryGetComponent(out IActivate iactivate))
                        {
                            iactivate.Activate();
                        }
                    }
                    spriteTransform.transform.position += new Vector3(0, -spriteMoveOffset,0);
                }
            }
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (Utility.LayerCheck(collision, triggerLayer))
        {
            if (objsOnPlatform.Contains(collision.gameObject) == true)
            {
                objsOnPlatform.Remove(collision.gameObject);
                if (objsOnPlatform.Count == 0)
                {
                    if (objsToControl == null) return;

                    foreach (GameObject obj in objsToControl)
                    {
                        if (obj.TryGetComponent(out IActivate iactivate))
                        {
                            iactivate.Deactivate();
                        }
                    }
                    spriteTransform.transform.position += new Vector3(0, spriteMoveOffset, 0);
                }
            }
        }
    }
}
