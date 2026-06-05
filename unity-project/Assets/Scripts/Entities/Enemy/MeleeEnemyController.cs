using UnityEngine;

[RequireComponent(typeof(MeleeAttackComponent))]
public class MeleeEnemyController : EnemyController
{
    private const string LayerNamePlayer = "Player";

    [Header("SO Data")]
    [SerializeField] protected MeleeEnemyData meleeEnemyData;

    protected override EnemyData Data => meleeEnemyData;

    protected MeleeAttackComponent meleeAttackComp;
    private IMeleeMovementStrategy movementStrategy;
    private bool telegraphShown = false;

    protected override void Awake()
    {
        if (meleeEnemyData == null)
        {
            enabled = false;
            return;
        }

        base.Awake();

        meleeAttackComp = GetComponent<MeleeAttackComponent>();
        meleeAttackComp.Init(meleeEnemyData, LayerNamePlayer, meleeEnemyData.name);

        movementStrategy = MovementStrategyFactory.Create(meleeEnemyData, MovementComp);
    }

    protected override void HandleMovement(Vector2 dir, float sqrDist)
    {
        Vector2 moveDir = movementStrategy.GetMoveDirection(dir, sqrDist, meleeEnemyData.circleStrafeDist);
        MovementComp.Move(moveDir);
    }

    protected override void HandleAttack(Vector2 dir)
    {
        if (!meleeAttackComp.CanAttack())
        {
            telegraphShown = false;
            return;
        }

        if (!telegraphShown)
        {
            ShowTelegraph();
            telegraphShown = true;
        }

        TriggerAttackAnimation();
        meleeAttackComp.Attack(dir);
    }
}