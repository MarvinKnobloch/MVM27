using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class CollapsingPlatform : MonoBehaviour
{
    private BoxCollider2D boxCollider;
    private SpriteRenderer spriteRenderer;
    private GameObject childObj;

    private bool collapsing;
    private int changeSpeedAmount = 3;
    private int speedChanges;

    [SerializeField] private float collapseTime;
    private float timer;
    [SerializeField] private float respawnTime;
    [SerializeField] private float baseBlinkSpeed;
    private float blinkSpeed;
    private float blinkTimer;
    private bool normalColor;
    private Color baseColor;

    [Space]
    [SerializeField] private GameManager.OverworldSaveNames saveName;
    private void Awake()
    {
        boxCollider = GetComponent<BoxCollider2D>();
        childObj = transform.GetChild(0).gameObject;
        spriteRenderer = childObj.GetComponent<SpriteRenderer>();
        float xsize = spriteRenderer.size.x;
        boxCollider.size = new Vector2(xsize * 0.98f, transform.GetChild(0).transform.localScale.y);  // 0.49f
        childObj.GetComponent<BoxCollider2D>().size = new Vector2(xsize, 1);

        baseColor = spriteRenderer.color;
    }

    private void Start()
    {
        if (saveName != GameManager.OverworldSaveNames.Empty)
        {
            if (GameManager.Instance.LoadProgress(saveName) == true) gameObject.SetActive(false);
        }         
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (collapsing == false)
            { 
                StartCoroutine(StartCollapsing());
                StartCoroutine(PlatformBlink());

                GameManager.Instance.SaveProgress(saveName);
            }
        }
    }
    IEnumerator StartCollapsing()
    {
        speedChanges = 0;
        collapsing = true;
        while (speedChanges < changeSpeedAmount)
        {
            yield return new WaitForSeconds(collapseTime / 3);
            blinkSpeed *= 0.5f;
            speedChanges++;
        }
        StopAllCoroutines();
        spriteRenderer.color = baseColor;
        normalColor = false;
        childObj.SetActive(false);
        boxCollider.enabled = false;
        if(respawnTime != 0) StartCoroutine(Respawn());
    }
    IEnumerator PlatformBlink()
    {
        blinkSpeed = baseBlinkSpeed;
        blinkTimer = 0;
        while (true)
        {
            blinkTimer += Time.deltaTime;
            if (blinkTimer >= blinkSpeed)
            {
                blinkTimer = 0;
                normalColor = !normalColor;
                if (normalColor) spriteRenderer.color = Color.red;
                else spriteRenderer.color = baseColor;
            }
            yield return null;
        }
    }
    IEnumerator Respawn()
    {
        yield return new WaitForSeconds(respawnTime);
        collapsing = false;
        boxCollider.enabled = true;
        childObj.SetActive(true);
    }
}
