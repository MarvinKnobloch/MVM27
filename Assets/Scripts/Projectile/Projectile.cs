using UnityEngine;
using System;
using Unity.VisualScripting;
using System.Collections;

public class Projectile : MonoBehaviour, IPoolingList
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

    public ObjectPooling.PoolObjectInfo poolingList { get; set; }

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }
    private void OnEnable()
    {
        oldPosition = transform.position;
        StartCoroutine(ProjectileDisable());
    }
    //void Start()
    //{
    //    oldPosition = transform.position;
    //    StartCoroutine(ProjectileDisable());
    //}
    private void FixedUpdate()
    {
        rb.linearVelocityY = 0;
        rb.transform.Translate(transform.right * projectileSpeed * Time.deltaTime, Space.World);

        direction = ((Vector2)transform.position - oldPosition).normalized;
        oldPosition = transform.position;
        transform.right = direction;
    }
    private IEnumerator ProjectileDisable()
    {
        yield return new WaitForSeconds(lifetime);
        ObjectPooling.ReturnObjectToPool(gameObject, poolingList);
    }

        
    public void Reflect()
    {
        transform.Rotate(0, 0, 180);
    }
    void OnTriggerEnter2D(Collider2D other)
    {
        // Enemy hit check
        if (Utility.LayerCheck(other, enemyHitLayer))
        {
            if(other.TryGetComponent(out Health health))
            {
                if(other.gameObject == Player.Instance.gameObject)
                {
                    health.PlayerTakeDamage(damage, false, false);
                }
                else
                {
                    health.EnemyTakeDamage(damage);
                }
            }
            ObjectPooling.ReturnObjectToPool(gameObject, poolingList);
            //Destroy(gameObject);
        }
        // Collide
        else if(Utility.LayerCheck(other, collideLayer))
        {
            {
                ObjectPooling.ReturnObjectToPool(gameObject, poolingList);
                //Destroy(gameObject);
            }
        }
        //Burn hit check
        else if(Utility.LayerCheck(other, burnLayer))
        {
            if (other.gameObject.TryGetComponent(out ObjectBurn objectBurn))
            {
                objectBurn.BurningStart();
            }
            else
            {
                Destroy(other.gameObject);
            }
            ObjectPooling.ReturnObjectToPool(gameObject, poolingList);
            //Destroy(gameObject);
        }
        //Reflect
        else if (Utility.LayerCheck(other, reflectLayer))
        {
            if (other.gameObject.TryGetComponent(out Reflectable reflectable))
            {
                reflectable.ReflectProjectile();

                ObjectPooling.ReturnObjectToPool(gameObject, poolingList);
                //Destroy(gameObject);
            }
        }
    }
}
