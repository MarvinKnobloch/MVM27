using System;
using UnityEngine;

public class Reflectable : MonoBehaviour
{
    private Projectile projectile;
    [NonSerialized] public bool isReflected;
    [SerializeField] ReflectableType type;

    [NonSerialized] public Animator animator;
    [NonSerialized] public string currentstate;

    enum ReflectableType
    {
        Empty,
        Tornado,
    }

    private void Awake()
    {
        projectile = GetComponent<Projectile>();
    }
    public void ReflectProjectile()
    {
        isReflected = true;
        projectile.Reflect();

        if (type == ReflectableType.Tornado)
        {
            animator = GetComponent<Animator>();
            ChangeAnimationState("ProjectileTranstion");
            AudioManager.Instance.PlayAudioFileOneShot(AudioManager.Instance.airBossSounds[(int)AudioManager.AirBossSounds.AirbossFireTornado]);
        }
    }
    public void FinalProjectileAnimation()
    {
        ChangeAnimationState("FinalProjectile");
    }

    public void ChangeAnimationState(string newstate)
    {
        if (currentstate == newstate) return;
        currentstate = newstate;
        if (animator == null) return;

        animator.CrossFadeInFixedTime(newstate, 0.1f);
    }
}
