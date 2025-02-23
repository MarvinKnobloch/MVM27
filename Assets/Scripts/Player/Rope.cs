using UnityEngine;

public class Rope : MonoBehaviour
{
    private Rigidbody2D rb;
    public float swayForce = 5f;
    public float swayFrequency = 2f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        if (rb == null)
        {
            Debug.LogError("Rope needs ridgidbody");
        }
        else
        {
            rb.bodyType = RigidbodyType2D.Dynamic;
            rb.gravityScale = 0;
        }
    }

    void FixedUpdate()
    {
        rb.AddForce(new Vector2(Mathf.Sin(Time.time * swayFrequency) * swayForce, 0));
    }
}
