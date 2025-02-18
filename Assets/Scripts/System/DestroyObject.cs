using UnityEngine;

public class DestroyObject : MonoBehaviour
{
    [SerializeField] private float lifeTime;

    private void Awake()
    {
        Destroy(gameObject, lifeTime);
    }
}
