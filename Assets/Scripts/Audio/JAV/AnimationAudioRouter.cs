using UnityEngine;

public class AnimationAudioRouter : MonoBehaviour
{
    public void PlayAnimationSound(string eventKey)
    {
        WwiseAudioManager.Instance.TriggerEvent(eventKey, gameObject);
    }
}