using UnityEngine;

[RequireComponent(typeof(ProjectileAttackComponent))]
public class RangedEnemyController : EnemyController
{
    [Header("SO Data")]
    [SerializeField] private RangedEnemyData rangedData;

    protected override EnemyData Data => rangedData;

    private ProjectileAttackComponent projectileAttackComp;

    protected override void Awake()
    {
        base.Awake();

        projectileAttackComp = GetComponent<ProjectileAttackComponent>();
        if (projectileAttackComp != null && rangedData.projectileData != null)
            projectileAttackComp.Init(rangedData.projectileData, bonusDamage: rangedData.damage);
    }

    protected override void HandleAttack(Vector2 dir)
    {
        projectileAttackComp?.Attack(dir);
    }
}