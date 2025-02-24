using UnityEngine;
//this code is for the bullet damage section of the enemy code
[System.Serializable]
public class BulletDamageData
{
    public GameObject bulletPrefab; // Selecdt bullet, must be prefab
    public int damageAmount;        // set bullet damage
}
