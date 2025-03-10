using UnityEngine;

public class SwitchPlatform : MonoBehaviour
{
    [SerializeField] private bool onWaySwitch;
    [SerializeField] private MovingPlatform[] platforms;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            foreach (var platform in platforms)
            {
                if (onWaySwitch) platform.SwitchPlatformUseabiltity(true);
                else platform.SwitchPlatformUseabiltity(false);
            }
        }
    }
}
