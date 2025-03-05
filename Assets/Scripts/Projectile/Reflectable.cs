using UnityEngine;

public class Reflectable : MonoBehaviour
{
    private Projectile projectile;

    private void Awake()
    {
        projectile = GetComponent<Projectile>();
    }
    public void Reflect()
    {
        projectile.projectileSpeed *= -1;
    }
}
