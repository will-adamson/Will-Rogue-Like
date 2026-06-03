using UnityEngine;

[RequireComponent(typeof(MoveComponent), typeof(PlayerDetectorComponent))]
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
        if (IsDead || PlayerDetectorComp == null)
        {
            MovementComp.Move(Vector2.zero);
            return;
        }

        if (!PlayerDetectorComp.IsTargetDetected(out Vector2 dir, out float sqrDist))
        {
            MovementComp.Move(Vector2.zero);
            return;
        }

        Sprite.flipX = dir.x < 0f;

        bool inAttackRange = PlayerDetectorComp.IsInAttackRange(sqrDist);

        if (!inAttackRange && !Data.isHoldPosition)
            HandleMovement(dir, sqrDist);
        else
            MovementComp.Move(Vector2.zero);

        if (inAttackRange)
            HandleAttack(dir);
    }

    protected virtual void HandleMovement(Vector2 dir, float sqrDist)
    {
        MovementComp.Move(dir);
    }

    protected abstract void HandleAttack(Vector2 dir);

    protected void TriggerAttackAnimation() => Anim.SetTrigger(HashAttack);

    protected override void HandleDeath()
    {
        Anim.SetTrigger(HashDie);
        Destroy(gameObject, GetDeathClipLength());
        HUDController.Instance?.LogFeed.LogSystem($" {Data.name} was defeated.");
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