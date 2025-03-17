using UnityEngine;
using UnityEngine.Events;

// Just passes trigger events up
public class CrawlerDamageCollider : MonoBehaviour
{
    public UnityAction<Collider2D> OnTriggerEnter;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        OnTriggerEnter?.Invoke(collision);
    }
}
