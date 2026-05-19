using UnityEngine;

[RequireComponent(typeof(MoveComponent))]
[RequireComponent(typeof(PlayerDetectorComponent))]
public abstract class EnemyController : EntityController
{
    protected abstract EnemyData Data { get; }

    protected MoveComponent MovementComp { get; private set; }
    protected PlayerDetectorComponent PlayerDetectorComp { get; private set; }

    protected override float MaxHealth => Data.health;
    protected override float GetDefence() => Data.defence;

    protected override void Awake()
    {
        base.Awake();

        MovementComp = GetComponent<MoveComponent>();
        MovementComp.Init(Rb, Data.speed);

        PlayerDetectorComp = GetComponent<PlayerDetectorComponent>();
        PlayerDetectorComp.Init(Data.detectionRange, Data.attackRange);
    }

    private void Update()
    {
        if (IsDead || PlayerDetectorComp == null) return;
        if (!PlayerDetectorComp.IsTargetDetected(out Vector2 dir, out float sqrDist)) return;

        Sprite.flipX = dir.x < 0f;

        bool inAttackRange = PlayerDetectorComp.IsInAttackRange(sqrDist);

        if (!inAttackRange && !Data.isHoldPosition)
            MovementComp.Move(dir);

        if (inAttackRange)
            HandleAttack(dir);
    }

    protected abstract void HandleAttack(Vector2 dir);

    protected override void HandleDeath() => Destroy(gameObject);
}