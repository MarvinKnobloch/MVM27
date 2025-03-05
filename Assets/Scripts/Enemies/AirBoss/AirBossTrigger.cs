using UnityEngine;

public class AirBossTrigger : MonoBehaviour
{
    private AirBoss airBoss;

    private void Awake()
    {
        airBoss = GetComponentInParent<AirBoss>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.TryGetComponent(out Reflectable reflectable))
        {
            if (reflectable.isReflected)
            {
                airBoss.ReflectHit();
                Destroy(collision.gameObject);
            }
        }
        if(collision.gameObject == Player.Instance.gameObject)
        {
            Player.Instance.health.TakeDamage(airBoss.chargeDamage, false);
        }
    }
}
