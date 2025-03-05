using System;
using UnityEngine;

public class Reflectable : MonoBehaviour
{
    private Projectile projectile;
    [NonSerialized] public bool isReflected;

    private void Awake()
    {
        projectile = GetComponent<Projectile>();
    }
    public void Reflect()
    {
        isReflected = true;
        projectile.Reflect();
    }
}
