using UnityEngine;

[RequireComponent(typeof(MeleeAttackComponent))]
public class MeleeEnemyController : EnemyController
{
    private const string LAYER_NAME_PLAYER = "Player";

    [Header("SO Data")]
    [SerializeField] private MeleeEnemyData meleeData;

    protected override EnemyData Data => meleeData;

    private MeleeAttackComponent meleeAttackComp;

    protected override void Awake()
    {
        base.Awake();

        meleeAttackComp = GetComponent<MeleeAttackComponent>();
        meleeAttackComp.Init(meleeData.damage, meleeData.knockbackForce, meleeData.attackCooldown, LAYER_NAME_PLAYER);
    }

    protected override void HandleAttack(Vector2 dir)
    {
        if (meleeAttackComp != null && meleeAttackComp.Attack(dir))
            TriggerAttackAnimation();
    }
}