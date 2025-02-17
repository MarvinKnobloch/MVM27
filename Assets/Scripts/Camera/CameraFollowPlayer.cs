using UnityEngine;
//camera setting In was working on to follow player, might scrap it. Not to happy with it.
public class CameraFollowPlayer : MonoBehaviour
{
    [Header("Target Settings")]
    // set target
    public Transform target;

    [Header("Smooth Follow Settings")]
    // safezone spped
    public float smoothTime = 0.3f;
    // offset
    public Vector3 offset = new Vector3(0, 2, -10f);

    [Header("Safe Zone Settings")]
    // Define safe zone
    [Range(0f, 1f)] public float minX = 0.3f;
    [Range(0f, 1f)] public float maxX = 0.7f;
    [Range(0f, 1f)] public float minY = 0.3f;
    [Range(0f, 1f)] public float maxY = 0.7f;

    [Header("Catch-Up Settings")]
    // dangerzone time
    public float catchUpSmoothTime = 0.1f;

    // Velocity sum
    private Vector3 velocity = Vector3.zero;
    private Camera cam;

    void Start()
    {
        cam = Camera.main;
    }

    void LateUpdate()
    {
        if (target == null)
            return;

        // align camera to target
        Vector3 targetPosition = target.position + offset;

        // Convert the target position to viewport.
        Vector3 viewportPos = cam.WorldToViewportPoint(target.position);

        // check in safe zone
        bool outsideHorizontal = viewportPos.x < minX || viewportPos.x > maxX;
        bool outsideVertical = viewportPos.y < minY || viewportPos.y > maxY;

        // set smoothness
        float currentSmoothTime = (outsideHorizontal || outsideVertical) ? catchUpSmoothTime : smoothTime;

        // move
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, currentSmoothTime);
    }
}
