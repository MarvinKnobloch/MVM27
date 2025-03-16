using System;
using UnityEngine;

public class EnemyDrops : MonoBehaviour
{
    [SerializeField] private Drops[] dropsOnDeath;

    public void DropsOnDeath()
    {
        for (int i = 0; i < dropsOnDeath.Length; i++)
        {
            if (dropsOnDeath[i].dropChance >= UnityEngine.Random.Range(0, 100))
            {
                GameObject drop = Instantiate(dropsOnDeath[i].objectToDrop, transform.position + (Vector3.up * 1), Quaternion.identity);
                if(drop.transform.GetChild(0).TryGetComponent(out Collectable collectable))
                {
                    collectable.SetValue(dropsOnDeath[i].objectValue);
                }
            }
        }
    }

    [Serializable]
    public struct Drops
    {
        [Range(1, 100)] public int dropChance;
        public GameObject objectToDrop;
        public int objectValue;
    }
}
