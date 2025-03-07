using UnityEngine;

public class FlyerSingleShot : FlyerAttack
{
    public override void Cast()
    {
        Vector2 targetPosition = initalTargetPosition;
        if (customTargetPosition != Vector2.zero)
            targetPosition = customTargetPosition;

        Vector2 direction = (targetPosition - rb.position).normalized;
        //float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        //transform.rotation = Quaternion.Euler(0f, 0f, angle);
        rb.linearVelocity = direction * speed;
    }
}