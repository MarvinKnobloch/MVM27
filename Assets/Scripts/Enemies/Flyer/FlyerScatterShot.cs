using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The scatter shot basically shoots multiple singleshots at different angles, towards the player
/// </summary>
public class FlyerScatterShot : FlyerAttack
{
    [Header("Shot Config")]
    [Tooltip("This is the prefab for each individual bullet")]
    [SerializeField] private FlyerAttack shotPrefab;
    [SerializeField, Min(1)] private int numberOfShots = 4;
    [SerializeField, Min(0f)] private float shotSpread = 1.2f;

    List<FlyerAttack> shotObjects = new List<FlyerAttack>();

    protected override void Awake()
    {
        // dont call away because we do not need the rigidbody
        //base.Awake();

        if (shotPrefab == null)
            throw new System.ArgumentException(nameof(shotPrefab));
    }

    public override void Init(Transform targetTransform)
    {
        base.Init(targetTransform);

        Vector2 baseDirection = (initalTargetPosition - (Vector2)transform.position).normalized;

        for (int i = 0; i < numberOfShots; i++)
        {
            Vector2 shotTargetPosition;

            if (i == 0)
                shotTargetPosition = initalTargetPosition;
            else
            {
                Vector2 randomOffset = new Vector2(
                   Random.Range(-shotSpread, shotSpread),
                   Random.Range(-shotSpread, shotSpread)
               );
                shotTargetPosition = initalTargetPosition + randomOffset;
            }

            // 
            FlyerAttack shot = Instantiate(shotPrefab, transform);
            shot.Init(target);
            shot.customTargetPosition = shotTargetPosition;
            shotObjects.Add(shot);
        }
    }

    public override void Cast()
    {
        foreach (var shot in shotObjects)
            shot.Cast();
    }
}
