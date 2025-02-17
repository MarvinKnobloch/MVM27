using UnityEngine;

public class Bullet : MonoBehaviour
{
    // destroy bullet time
    public float lifetime = 2f;

    void Start()
    {
        Destroy(gameObject, lifetime);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Bullet entered with " + other.gameObject.name);

        // hit check
        if (other.CompareTag("Enemy"))
        {
            FlyingEnemy enemy = other.GetComponent<FlyingEnemy>();
            if (enemy != null)
            {
                enemy.OnHitByBullet(gameObject);
            }

            // Destroy bullet after hit
            Destroy(gameObject);
        }
    }
}
