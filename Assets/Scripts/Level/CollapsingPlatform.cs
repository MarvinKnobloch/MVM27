using System.Collections;
using UnityEngine;

public class CollapsingPlatform : MonoBehaviour
{
    private BoxCollider2D boxCollider;
    private bool collapsing;
    private SpriteRenderer spriteRenderer;
    private int changeSpeedAmount = 3;
    private int speedChanges;

    [SerializeField] private float collapseTime;
    private float timer;
    [SerializeField] private float blinkSpeed;
    private float blinkTimer;
    private bool normalColor;
    private void Awake()
    {
        boxCollider = GetComponent<BoxCollider2D>();
        spriteRenderer = transform.GetChild(0).GetComponent<SpriteRenderer>();
        float xsize = spriteRenderer.size.x;
        boxCollider.size = new Vector2(xsize * 0.98f, transform.GetChild(0).transform.localScale.y);  // 0.49f
        transform.GetChild(0).GetComponent<BoxCollider2D>().size = new Vector2(xsize, 1);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (collapsing == false)
            { 
                StartCoroutine(StartCollapsing());
                StartCoroutine(PlatformBlink());
            }
        }
    }
    IEnumerator StartCollapsing()
    {
        collapsing = true;
        while (speedChanges < changeSpeedAmount)
        {
            yield return new WaitForSeconds(collapseTime / 3);
            blinkSpeed *= 0.5f;
            speedChanges++;
        }
        StopAllCoroutines();
        Destroy(gameObject);
    }
    IEnumerator PlatformBlink()
    {
        while (true)
        {
            blinkTimer += Time.deltaTime;
            if (blinkTimer >= blinkSpeed)
            {
                blinkTimer = 0;
                normalColor = !normalColor;
                if (normalColor) spriteRenderer.color = Color.red;
                else spriteRenderer.color = Color.white;
            }
            yield return null;
        }
    }
}
