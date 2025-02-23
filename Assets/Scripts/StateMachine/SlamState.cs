using UnityEngine;

public class SlamState : EnemyState
{
    public override void EnterState(FlyingEnemy enemy)
    {
        Debug.Log("Entering Slam State");

        enemy.damageCollider.enabled = false;
        enemy.spriteObject.SetActive(false);

        enemy.GetComponent<Rigidbody2D>().linearVelocity = Vector2.zero;

        enemy.GetComponent<Rigidbody2D>().linearVelocity = new Vector2(0, -enemy.slamSpeed);
    }

    public override void UpdateState(FlyingEnemy enemy)
    {
        if (enemy.transform.position.y <= enemy.groundY - 0.43f)
        {
            enemy.transform.position = new Vector3(enemy.transform.position.x, enemy.groundY - 0.42f, enemy.transform.position.z);

            enemy.GetComponent<Rigidbody2D>().linearVelocity = Vector2.zero;

            Debug.Log("Enemy hit the ground! Staying for " + enemy.slamStayTime + " seconds.");

            enemy.StartCoroutine(enemy.ReturnToOriginalHeight());
        }
    }

    public override void ExitState(FlyingEnemy enemy)
    {
        Debug.Log("Exiting Slam State");
    }
}
