using UnityEngine;

[RequireComponent(typeof(MovementComponent), typeof(ProjectileAttackComponent), typeof(PlayerDetectorComponent))]
public class EnemyController : EntityController
{
    [Header("SO Data")]
    [SerializeField] private EnemyData enemyData;

    private MovementComponent movementComp;
    private ProjectileAttackComponent projectileAttackComp;
    private PlayerDetectorComponent playerDetectorComp;

    protected override float MaxHealth => enemyData.health;
    protected override float GetDefence() => enemyData.defence;

    protected override void Awake()
    {
        base.Awake();

        movementComp = GetComponent<MovementComponent>();
        projectileAttackComp = GetComponent<ProjectileAttackComponent>();
        playerDetectorComp = GetComponent<PlayerDetectorComponent>();

        movementComp.Init(Rb, enemyData.speed);

        if (projectileAttackComp != null && enemyData.projectileData != null)
            projectileAttackComp.Init(enemyData.projectileData, bonusDamage: enemyData.damage);

        playerDetectorComp.Init(enemyData.detectionRange, enemyData.attackRange);

        if (enemyData.sprite != null) Sprite.sprite = enemyData.sprite;
    }

    private void Update()
    {
        if (IsDead || playerDetectorComp == null) return;
        if (!playerDetectorComp.IsTargetDetected(out Vector2 dir, out float sqrDist)) return;

        Sprite.flipX = dir.x < 0f;

        bool inAttackRange = playerDetectorComp.IsInAttackRange(sqrDist);

        if (!inAttackRange && !enemyData.isHoldPosition) movementComp.Move(dir);

        if (inAttackRange)
        {
            if (projectileAttackComp != null) projectileAttackComp.Attack(dir);
        }
    }

    protected override void HandleDeath()
    {
        Destroy(gameObject);
    }
}