using Mono.Cecil.Cil;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public abstract class FlyerAttack : MonoBehaviour
{
    [SerializeField] protected Rigidbody2D rb;
    [SerializeField, Min(0)] protected int damage = 1;
    [SerializeField, Min(0)] protected int speed = 5;

    protected Transform target;
    protected Vector2 initalTargetPosition;

    protected virtual void Awake()
    {
        
        if (rb == null)
            rb = GetComponent<Rigidbody2D>();
    }

    public virtual void Init(Transform targetTransform)
    {
        this.target = targetTransform;
        initalTargetPosition = (Vector2)targetTransform.position;
    }

    public abstract void Cast();
}
