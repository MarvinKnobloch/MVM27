using UnityEngine;

public class SortingLayer : MonoBehaviour
{
    void Start()
    {
        transform.GetComponent<Renderer>().sortingLayerName = "Background";
    }
}
