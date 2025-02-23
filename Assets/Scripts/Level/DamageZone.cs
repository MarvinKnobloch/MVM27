using UnityEngine;

public class DamageZone : MonoBehaviour
{
    [SerializeField] private int damage;
    [SerializeField] private Zone zone;
    public enum Zone
    {
        NormalZone,
        FireZone,
        LavaZone,
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            ZoneInteraction();
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            ZoneInteraction();
        }
    }
    private void ZoneInteraction()
    {
        switch (zone)
        {
            case Zone.NormalZone:
                Player.Instance.health.TakeDamage(damage);
                break;
            case Zone.FireZone:
                if (Player.Instance.currentElementNumber != 1) Player.Instance.health.TakeDamage(damage);
                break;
            case Zone.LavaZone:
                if (Player.Instance.currentElementNumber == 1 && Player.Instance.state == Player.States.Dash) return;

                Player.Instance.health.TakeDamage(damage);
                break;
                
        }
    }
}
