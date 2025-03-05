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
    [SerializeField] public float projectileSpeed;
    [SerializeField] private LayerMask collideLayer;
    

    [Header("EnemyValues")]
    [SerializeField] private LayerMask enemyHitLayer;
    [SerializeField] private int damage;

    [Header("BurnObjects")]
    [SerializeField] private LayerMask burnLayer;
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
    void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Bullet entered with " + other.gameObject.name);

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
            if(other.gameObject.TryGetComponent(out Reflectable reflectable))
            { 

            }
            Destroy(gameObject);
        }
        //Burn hit check
        else if(Utility.LayerCheck(other, burnLayer))
        {
            Destroy(other.gameObject);
            Destroy(gameObject);
        }
    }
}
