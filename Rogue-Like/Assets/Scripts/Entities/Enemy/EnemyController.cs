using UnityEngine;

[RequireComponent(typeof(MoveComponent))]
[RequireComponent(typeof(PlayerDetectorComponent))]
public abstract class EnemyController : EntityController
{
    private static readonly int HashAttack = Animator.StringToHash("Attack");
    private static readonly int HashDie = Animator.StringToHash("Die");

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

    protected void TriggerAttackAnimation()
    {
        Anim.SetTrigger(HashAttack);
    }

    protected override void HandleDeath()
    {
        Anim.SetTrigger(HashDie);
        Destroy(gameObject, GetDeathClipLength());
    }

    private float GetDeathClipLength()
    {
        foreach (AnimationClip clip in Anim.runtimeAnimatorController.animationClips)
            if (clip.name.IndexOf("die", System.StringComparison.OrdinalIgnoreCase) >= 0 ||
                clip.name.IndexOf("death", System.StringComparison.OrdinalIgnoreCase) >= 0)
                return clip.length;
        return 1f;
    }
}