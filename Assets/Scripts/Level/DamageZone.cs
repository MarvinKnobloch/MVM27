using Unity.VisualScripting;
using UnityEngine;

public class DamageZone : MonoBehaviour
{
    [SerializeField] private int damage;
    [SerializeField] private Zone zone;
    [SerializeField] private bool lavaIgnoreIframes;

    private BoxCollider2D boxCollider;
    public enum Zone
    {
        NormalZone,
        FireZone,
        LavaZone,
    }
    private void Awake()
    {
        if(zone == Zone.LavaZone)
        {
            boxCollider = GetComponent<BoxCollider2D>();
            boxCollider.isTrigger = false;
        }

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
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject == Player.Instance.gameObject)
        {
            if (zone == Zone.LavaZone)
            {
                if (Player.Instance.currentElementNumber == 1 && Player.Instance.state == Player.States.Dash)
                {
                    boxCollider.isTrigger = true;
                }
                else
                {
                    if (lavaIgnoreIframes) Player.Instance.health.PlayerTakeDamage(damage, false, false);
                    else Player.Instance.health.PlayerTakeDamage(damage, false, false);
                }
            }
        }
    }
    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject == Player.Instance.gameObject)
        {
            if (zone == Zone.LavaZone)
            {
                if (Player.Instance.currentElementNumber == 1 && Player.Instance.state == Player.States.Dash)
                {
                    boxCollider.isTrigger = true;
                }
                else
                {
                    if (lavaIgnoreIframes) Player.Instance.health.PlayerTakeDamage(damage, false, false);
                    else Player.Instance.health.PlayerTakeDamage(damage, false, false);
                }
            }
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (zone == Zone.LavaZone)
            {
                boxCollider.isTrigger = false;
            }
        }
    }
    private void ZoneInteraction()
    {
        switch (zone)
        {
            case Zone.NormalZone:
                Player.Instance.health.PlayerTakeDamage(damage, false, false);
                break;
            case Zone.FireZone:
                if (Player.Instance.currentElementNumber != 1) Player.Instance.health.PlayerTakeDamage(damage, false, false);
                break;
            case Zone.LavaZone:
                if (Player.Instance.currentElementNumber == 1 && Player.Instance.state == Player.States.Dash) return;

                if(lavaIgnoreIframes) Player.Instance.health.PlayerTakeDamage(damage, false, false);
                else Player.Instance.health.PlayerTakeDamage(damage, false, false);
                break;
                
        }
    }
}
