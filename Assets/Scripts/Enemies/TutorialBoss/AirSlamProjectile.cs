using System.Collections;
using UnityEngine;

public class AirSlamProjectile : MonoBehaviour
{
    [SerializeField] private float timeUntilDrop;
    [SerializeField] private int damage;
    [SerializeField] private LayerMask hitLayer;
    private void Start()
    {
        StartCoroutine(AddGravity());
    }
    IEnumerator AddGravity()
    {
        yield return new WaitForSeconds(timeUntilDrop);
        GetComponent<Rigidbody2D>().gravityScale = 1;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(Utility.LayerCheck(collision, hitLayer))
        {
            if (collision.CompareTag("Player"))
            {
                Player.Instance.health.TakeDamage(damage, false);
            }
            Destroy(gameObject);
        }
    }
}
