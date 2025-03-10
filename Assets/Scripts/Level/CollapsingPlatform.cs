using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class CollapsingPlatform : MonoBehaviour
{
    private BoxCollider2D boxCollider;
    private BoxCollider2D childCollider;
    private SpriteRenderer spriteRenderer;
    private GameObject childObj;
    private Animator animator;

    private bool collapsing;
    private int changeSpeedAmount = 3;
    private int speedChanges;

    [SerializeField] private float collapseTime;
    private float timer;
    [SerializeField] private float respawnTime;
    [SerializeField] private float baseBlinkSpeed;
    [SerializeField] private Sprite[] platformSprites;
    private float blinkSpeed;
    private float blinkTimer;
    private bool normalColor;

    [Space]
    [SerializeField] private GameManager.OverworldSaveNames saveName;
    private void Awake()
    {
        boxCollider = GetComponent<BoxCollider2D>();
        childObj = transform.GetChild(0).gameObject;
        spriteRenderer = childObj.GetComponent<SpriteRenderer>();
        animator = transform.GetChild(0).gameObject.GetComponent<Animator>();
        childCollider = childObj.GetComponent<BoxCollider2D>();

        float xsize = spriteRenderer.size.x - 0.05f;
        boxCollider.size = new Vector2(xsize * 0.98f, boxCollider.size.y);  // 0.49f
        childCollider.size = new Vector2(xsize, boxCollider.size.y);
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
        spriteRenderer.sprite = platformSprites[0];
        normalColor = false;
        boxCollider.enabled = false;
        childCollider.enabled = false;
        ActivateAnimator();
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
                if (normalColor) spriteRenderer.sprite = platformSprites[1];
                else spriteRenderer.sprite = platformSprites[0];
            }
            yield return null;
        }
    }
    IEnumerator Respawn()
    {
        yield return new WaitForSeconds(respawnTime);
        collapsing = false;
        boxCollider.enabled = true;
        childCollider.enabled = true;
        spriteRenderer.sprite = platformSprites[0];
        spriteRenderer.enabled = true;
        animator.enabled = false;
    }
    private void ActivateAnimator()
    {
        animator.enabled = true; 
        animator.Play("Break", -1, 0f);
        StartCoroutine(DeactivateAnimator());
    }
    IEnumerator DeactivateAnimator()
    {
        yield return new WaitForSeconds(0.6f);
        animator.enabled = false;
        spriteRenderer.enabled = false;
    }
}
