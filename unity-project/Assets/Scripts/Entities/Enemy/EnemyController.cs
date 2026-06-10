using UnityEngine;

[RequireComponent(typeof(MoveComponent), typeof(PlayerDetectorComponent))]
public abstract class EnemyController : EntityController
{
    protected abstract EnemyData Data { get; }

    protected MoveComponent MovementComp { get; private set; }
    protected PlayerDetectorComponent PlayerDetectorComp { get; private set; }

    protected override float MaxHealth => Data.health;
    protected override float GetDefence() => Data.defence;

    private static readonly int HashIsWalking = Animator.StringToHash("IsWalking");
    private static readonly int HashLastDirX = Animator.StringToHash("LastDirX");
    private static readonly int HashLastDirY = Animator.StringToHash("LastDirY");

    private Vector2 lastMoveDirection = Vector2.down;

    protected override void Awake()
    {
        base.Awake();

        MovementComp = GetComponent<MoveComponent>();
        MovementComp.Init(Rb, Data.speed);

        PlayerDetectorComp = GetComponent<PlayerDetectorComponent>();
        PlayerDetectorComp.Init(Data.detectionRange, Data.attackRange);
    }

    protected virtual void Update()
    {
        if (IsDead || PlayerDetectorComp == null)
        {
            MovementComp.Move(Vector2.zero);
            UpdateAnimationDirection(Vector2.zero);
            return;
        }

        if (!PlayerDetectorComp.IsTargetDetected(out Vector2 dir, out float sqrDist))
        {
            MovementComp.Move(Vector2.zero);
            UpdateAnimationDirection(Vector2.zero);
            return;
        }

        bool inAttackRange = PlayerDetectorComp.IsInAttackRange(sqrDist);

        if (!inAttackRange && !Data.isHoldPosition)
        {
            HandleMovement(dir, sqrDist);
            UpdateAnimationDirection(dir);
        }
        else
        {
            MovementComp.Move(Vector2.zero);
            UpdateAnimationDirection(Vector2.zero);
        }

        if (inAttackRange)
            HandleAttack(dir);
    }

    protected virtual void HandleMovement(Vector2 dir, float sqrDist)
    {
        MovementComp.Move(dir);
    }

    protected abstract void HandleAttack(Vector2 dir);

    protected void UpdateAnimationDirection(Vector2 moveDirection)
    {
        bool isMoving = moveDirection.sqrMagnitude > 0.01f;
        Anim.SetBool(HashIsWalking, isMoving);

        if (isMoving)
            lastMoveDirection = SnapToCardinal(moveDirection);

        Anim.SetFloat(HashLastDirX, lastMoveDirection.x);
        Anim.SetFloat(HashLastDirY, lastMoveDirection.y);

        Debug.Log($"DirX: {lastMoveDirection.x} DirY: {lastMoveDirection.y}");
    }

    private Vector2 SnapToCardinal(Vector2 dir)
    {
        if (Mathf.Abs(dir.x) >= Mathf.Abs(dir.y))
            return new Vector2(Mathf.Sign(dir.x), 0f);
        else
            return new Vector2(0f, Mathf.Sign(dir.y));
    }

    protected void TriggerAttackAnimation() => Anim.SetTrigger(HashAttack);

    protected void ShowTelegraph()
    {
        if (HUDController.Instance == null || !HUDController.Instance.IsReady) return;

        HUDController.Instance.Telegraph.Show(
            Data.name,
            Data.attackName,
            Data.telegraphDuration,
            Data.danger
        );
    }

    protected override void HandleDeath()
    {
        Anim.SetTrigger(HashDie);
        Destroy(gameObject, GetDeathClipLength());
        HUDController.Instance.LogFeed.LogSystem($" {Data.name} was defeated.");
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