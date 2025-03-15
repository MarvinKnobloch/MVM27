using System.Collections.Generic;
using UnityEngine;

public class PressurePlate : MonoBehaviour
{
    [SerializeField] private LayerMask triggerLayer;
    private List<GameObject> objsOnPlatform = new List<GameObject>();
    [SerializeField] private GameObject[] objsToControl;

    private Animator animator;
    private string currentstate;
    private void Awake()
    {
        animator = GetComponent<Animator>();
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
                    ChangeAnimationState("Pressed");
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
                    ChangeAnimationState("Release");
                }
            }
        }
    }
    public void ChangeAnimationState(string newstate)
    {
        if (currentstate == newstate) return;
        currentstate = newstate;
        if (animator == null) return;

        animator.CrossFadeInFixedTime(newstate, 0.1f);
    }
}
