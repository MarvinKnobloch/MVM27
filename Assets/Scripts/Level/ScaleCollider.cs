using UnityEngine;

public class ScaleCollider : MonoBehaviour
{
    [SerializeField] private float scaleOffset;

    private BoxCollider2D boxCollider;
    private void Awake()
    {
        boxCollider = GetComponent<BoxCollider2D>();
        float xsize = transform.GetComponent<SpriteRenderer>().size.x + scaleOffset;
        boxCollider.size = new Vector2(xsize, boxCollider.size.y);
    }
}
