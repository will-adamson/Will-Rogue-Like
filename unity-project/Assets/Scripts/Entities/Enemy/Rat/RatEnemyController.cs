using System.Collections;
using UnityEngine;

public class RatEnemyController : MeleeEnemyController
{
    [Header("Rat Behaviour")]
    [SerializeField] private float panicSpeedMultiplier = 1.75f;
    [SerializeField] private bool canPanic = true;
    [SerializeField] private bool isPackLeader = false;

    private EnemyState state = EnemyState.Idle;
    private bool isPanicking = false;
    private RatPackComponent packComp;

    protected override void Awake()
    {
        base.Awake();
        Health.OnHealthChanged += OnHealthChanged;

        if (isPackLeader)
        {
            packComp = gameObject.AddComponent<RatPackComponent>();
            packComp.Init(meleeEnemyData);
            RegisterWithPackMembers();
        }
    }

    protected override void Update()
    {
        if (packComp != null)
            meleeAttackComp.SetDamageMultiplier(packComp.GetDamageMultiplier());

        if (IsDead) return;

        bool playerDetected = PlayerDetectorComp.IsTargetDetected(
            out Vector2 dirToPlayer, out float sqrDist);

        switch (state)
        {
            case EnemyState.Idle:
                MovementComp.Move(Vector2.zero);
                if (playerDetected)
                {
                    TransitionToAlert();
                }
                break;

            case EnemyState.Alert:
                break;

            case EnemyState.Chase:
            case EnemyState.Attack:
                if (!playerDetected)
                {
                    state = EnemyState.Idle;
                    MovementComp.ResetSpeed();
                    return;
                }

                Sprite.flipX = dirToPlayer.x < 0f;
                bool inAttackRange = PlayerDetectorComp.IsInAttackRange(sqrDist);

                if (!inAttackRange && !Data.isHoldPosition)
                    HandleMovement(dirToPlayer, sqrDist);
                else
                    MovementComp.Move(Vector2.zero);

                if (inAttackRange)
                {
                    state = EnemyState.Attack;
                    HandleAttack(dirToPlayer);
                }
                else
                {
                    state = EnemyState.Chase;
                }
                break;
        }
    }

    private void TransitionToAlert()
    {
        state = EnemyState.Alert;
        MovementComp.Move(Vector2.zero);
        StartCoroutine(AlertRoutine());
    }

    private IEnumerator AlertRoutine()
    {
        yield return new WaitForSeconds(1.5f);
        MovementComp.ResetSpeed();
        state = EnemyState.Chase;
    }

    private void RegisterWithPackMembers()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(
            transform.position, 6f, LayerMask.GetMask("Enemy"));

        foreach (Collider2D col in hits)
        {
            if (col.TryGetComponent(out SmallRatNotifier notifier))
                notifier.RegisterLeader(packComp);
        }
    }

    private void OnHealthChanged(float current, float max)
    {
        if (!canPanic || isPanicking) return;
        if (current <= max * 0.5f)
        {
            isPanicking = true;
            MovementComp.SetSpeed(Data.speed * panicSpeedMultiplier);
        }
    }
}