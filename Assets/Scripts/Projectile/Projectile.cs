using UnityEngine;
using System;

public class Projectile : MonoBehaviour
{
    private Rigidbody2D rb;

    [Header("ProjectileValues")]
    [SerializeField] private float lifetime = 2f;
    [SerializeField] private float xForce;
    [SerializeField] private float yForce;
    [SerializeField] private LayerMask collideLayer;
    [NonSerialized] public bool reverseForce;

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
        if (reverseForce) xForce *= -1;
 
        rb.AddForce(new Vector2(xForce, yForce), ForceMode2D.Impulse);
        Destroy(gameObject, lifetime);
    }
    void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Bullet entered with " + other.gameObject.name);

        // Enemy hit check
        if (Utility.LayerCheck(other, enemyHitLayer))
        {
            if(other.TryGetComponent(out Health health))
            {
                health.TakeDamage(damage);
            }
            Destroy(gameObject);
        }
        //Burn hit check
        else if(Utility.LayerCheck(other, burnLayer))
        {
            Destroy(other.gameObject);
            Destroy(gameObject);
        }

        //Destroy if collide
        else if(Utility.LayerCheck(other, collideLayer))
        {
            Destroy(gameObject);
        }
    }
}
