using System.Collections;
using UnityEngine;

public class ObjectBurn : MonoBehaviour
{
    [SerializeField] private float burningDuration;
    [SerializeField] private GameObject burningEffect;
    [SerializeField] private int burningEffectsAmount;
    [SerializeField] private float burningXSpread;
    [SerializeField] private float burningYSpread;
    private bool isBurning;
    public void BurningStart()
    {
        if (isBurning) return;

        for (int i = 0; i < burningEffectsAmount; i++)
        {
            Vector3 spread = new Vector3(Random.Range(-burningXSpread, burningXSpread), Random.Range(-burningYSpread, burningYSpread), 0);
            Instantiate(burningEffect, transform.position + spread, Quaternion.identity, transform);
        }
        StartCoroutine(Burn());
    }
    IEnumerator Burn()
    {
        yield return new WaitForSeconds(burningDuration);
        Destroy(gameObject);
    }
}
