using UnityEngine;

public class ScaleHeight : MonoBehaviour
{
    [SerializeField] private float scaleOffset;
    [SerializeField] private GameObject top;
    [SerializeField] private GameObject bottom;
    [SerializeField] private float topAndBottomOffset;

    private BoxCollider2D boxCollider;
    private void Awake()
    {
        boxCollider = GetComponent<BoxCollider2D>();
        float ysize = transform.GetComponent<SpriteRenderer>().size.y + scaleOffset;
        boxCollider.size = new Vector2(boxCollider.size.x, ysize);

        if (top != null) top.transform.position = transform.position + Vector3.up * (ysize * 0.5f + topAndBottomOffset);
        if (bottom != null) bottom.transform.position = transform.position -Vector3.up * (ysize * 0.5f + topAndBottomOffset);
    }
}
