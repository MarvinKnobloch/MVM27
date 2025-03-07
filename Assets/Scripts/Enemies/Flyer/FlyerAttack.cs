using UnityEngine;

// TODO: Add support for the shots to pool

public abstract class FlyerAttack : MonoBehaviour
{
    [SerializeField] protected Rigidbody2D rb;
    [SerializeField, Min(0)] protected int damage = 1;
    [SerializeField, Min(0)] protected int speed = 15;
    [SerializeField, Min(0)] protected float maxLifetime = 5f;

    protected Transform target;
    protected Vector2 initalTargetPosition;
    [System.NonSerialized] public Vector2 customTargetPosition;

    protected virtual void Awake()
    {
        if (rb == null)
            throw new System.ArgumentNullException(nameof(rb));

        // TODO: check the rb layer mask
    }

    protected virtual void Start()
    {
        Destroy(this, maxLifetime);
    }

    public virtual void Init(Transform targetTransform)
    {
        target = targetTransform;
        initalTargetPosition = (Vector2)targetTransform.position;
    }

    public abstract void Cast();

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // TODO: Remove log once we are happy with the flyer
        Debug.Log($"FLyerAttack::OnCollisionEnter2D hit {collision.gameObject.name}");
        Destroy(gameObject);
    }
}
