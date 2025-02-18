using System.Collections.Generic;
using UnityEngine;

public class PressurePlate : MonoBehaviour
{
    [SerializeField] private LayerMask triggerLayer;
    private List<GameObject> objsOnPlatform = new List<GameObject>();
    [SerializeField] private GameObject objToControl;

    private Transform spriteTransform;
    private float spriteMoveOffset = 0.2f;
    private void Awake()
    {
        spriteTransform = transform.GetChild(0).transform;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (((1 << collision.gameObject.layer) & triggerLayer) != 0)
        {
            if(objsOnPlatform.Contains(collision.gameObject) == false)
            {
                objsOnPlatform.Add(collision.gameObject);
                if(objsOnPlatform.Count == 1)
                {
                    if(objToControl.TryGetComponent(out IActivate iactivate))
                    {
                        iactivate.Activate();
                    }
                    spriteTransform.transform.position += new Vector3(0, -spriteMoveOffset,0);
                }
            }
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (((1 << collision.gameObject.layer) & triggerLayer) != 0)
        {
            if (objsOnPlatform.Contains(collision.gameObject) == true)
            {
                objsOnPlatform.Remove(collision.gameObject);
                if (objsOnPlatform.Count == 0)
                {
                    if (objToControl.TryGetComponent(out IActivate iactivate))
                    {
                        iactivate.Deactivate();
                    }
                    spriteTransform.transform.position += new Vector3(0, spriteMoveOffset, 0);
                }
            }
        }
    }
}
