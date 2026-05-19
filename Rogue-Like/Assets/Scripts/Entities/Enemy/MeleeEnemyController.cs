using UnityEngine;

public class MeleeEnemyController : EnemyController
{
    [Header("SO Data")]
    [SerializeField] private MeleeEnemyData meleeData;

    protected override EnemyData Data => meleeData;

    private MeleeAttackComponent meleeAttackComp;

    protected override void Awake()
    {
        base.Awake();

        meleeAttackComp = GetComponent<MeleeAttackComponent>();
        meleeAttackComp?.Init(meleeData.damage, meleeData.knockbackForce, meleeData.attackCooldown);
    }

    protected override void HandleAttack(Vector2 dir)
    {
        meleeAttackComp?.Attack(dir);
    }
}