using UnityEngine;

public class PlayerAnimationAudioBridge : MonoBehaviour
{
    [SerializeField] private PostWwiseEvent playerAudioSystem;

    public void PlayTaggedEvent(string key)
    {
        if (playerAudioSystem != null)
        {
            playerAudioSystem.PlayTaggedEvent(key);
        }
    }
}