using UnityEngine;

public class NormalPlatform : MonoBehaviour
{
    private BoxCollider2D boxCollider;
    private void Awake()
    {
        boxCollider = GetComponent<BoxCollider2D>();
        float xsize = transform.GetComponent<SpriteRenderer>().size.x - 0.8f;
        boxCollider.size = new Vector2(xsize, boxCollider.size.y);
    }
}
