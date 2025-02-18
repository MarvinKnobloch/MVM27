using UnityEditor;
using UnityEngine;

public class Destructable : MonoBehaviour
{
    [SerializeField] private GameObject destroyPrefab;
    [SerializeField] private int prefabCount;
    [Space]
    [SerializeField] private float YSpawnOffset;
    [SerializeField] private float XForce;
    [SerializeField] private float YForce;
    [SerializeField] private float randomForce;
    public void Interaction(Transform interactionTransform)
    { 
        if(destroyPrefab != null)
        {
            for (int i = 0; i < prefabCount; i++)
            {
                float ySpawn = Random.Range(-YSpawnOffset, YSpawnOffset);
                GameObject obj = Instantiate(destroyPrefab, transform.position + (ySpawn * transform.up), Quaternion.identity);

                float x = XForce + Random.Range(-randomForce, randomForce);
                float y = YForce + Random.Range(-randomForce, randomForce);
                if(interactionTransform.position.x > transform.position.x)
                {
                    obj.GetComponent<Rigidbody2D>().AddForce(new Vector2(-x, y), ForceMode2D.Impulse);
                }
                else obj.GetComponent<Rigidbody2D>().AddForce(new Vector2(x, y), ForceMode2D.Impulse);
            }
        }
        Destroy(gameObject);
    }
}
