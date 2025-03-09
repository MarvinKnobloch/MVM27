using Unity.Cinemachine;
using UnityEngine;

public class SetGameManagerCamera : MonoBehaviour
{
    void Start()
    {
        if (GameManager.Instance != null) GameManager.Instance.cinemachineCamera = GetComponent<CinemachineCamera>();
    }
}
