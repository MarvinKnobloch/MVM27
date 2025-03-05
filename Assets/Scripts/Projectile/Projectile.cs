using UnityEngine;
using System;
using Unity.VisualScripting;

public class Projectile : MonoBehaviour
{
    private Rigidbody2D rb;
    private Vector2 direction;
    private Vector2 oldPosition;

    [Header("ProjectileValues")]
    [SerializeField] private float lifetime = 2f;
    public float projectileSpeed;
    [SerializeField] private LayerMask collideLayer;
    

    [Header("EnemyValues")]
    [SerializeField] private LayerMask enemyHitLayer;
    [SerializeField] private int damage;

    [Header("BurnObjects")]
    [SerializeField] private LayerMask burnLayer;

    [Header("ReflectLayer")]
    [SerializeField] private LayerMask reflectLayer;

    private bool dontupdate;
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }
    void Start()
    {
        oldPosition = transform.position;
        Destroy(gameObject, lifetime);
    }
    private void FixedUpdate()
    {
        rb.linearVelocityY = 0;
        rb.transform.Translate(transform.right * projectileSpeed * Time.deltaTime, Space.World);

        direction = ((Vector2)transform.position - oldPosition).normalized;
        oldPosition = transform.position;
        transform.right = direction;
    }
    public void Reflect()
    {
        transform.Rotate(0, 0, 180);
        //oldPosition = (Vector2)transform.position + direction;
        //direction = ((Vector2)transform.position - oldPosition).normalized;
        //transform.right = direction;
        //dontupdate = true;


        //direction = (oldPosition - (Vector2)transform.position).normalized;
        //oldPosition = transform.position;
        //oldPosition = (Vector2)transform.position + direction;
    }
    void OnTriggerEnter2D(Collider2D other)
    {
        // Enemy hit check
        if (Utility.LayerCheck(other, enemyHitLayer))
        {
            if(other.TryGetComponent(out Health health))
            {
                health.TakeDamage(damage, false);
            }
            Destroy(gameObject);
        }
        // Collide
        else if(Utility.LayerCheck(other, collideLayer))
        {
            {
                Destroy(gameObject);
            }
        }
        //Burn hit check
        else if(Utility.LayerCheck(other, burnLayer))
        {
            Destroy(other.gameObject);
            Destroy(gameObject);
        }
        //Reflect
        else if (Utility.LayerCheck(other, reflectLayer))
        {
            if (other.gameObject.TryGetComponent(out Reflectable reflectable))
            {
                reflectable.Reflect();
                Destroy(gameObject);
            }
        }
    }
}
