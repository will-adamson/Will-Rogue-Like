using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(ProjectileAttackComponent))]
public class MagePlayerController : PlayerController
{
    private MagePlayerData magePlayerData;
    private InputAction attackAction;
    protected bool IsAttacking { get; set; }

    protected override void Awake()
    {
        base.Awake();

        magePlayerData = playerData as MagePlayerData;
        if (magePlayerData == null) return;

        ProjectileAttackComponent projectileAttackComp = GetComponent<ProjectileAttackComponent>();
        projectileAttackComp.Init(magePlayerData.projectileData, bonusDamage: magePlayerData.damage);

        Attacker = projectileAttackComp;
        attackAction = PlayerActionMap.FindAction("Attack");
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        if (attackAction == null) return;
        attackAction.Enable();
        attackAction.performed += OnAttackPerformed;
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        if (attackAction == null) return;
        attackAction.Disable();
        attackAction.performed -= OnAttackPerformed;
    }

    private void OnAttackPerformed(InputAction.CallbackContext ctx)
    {
        if (IsDead) return;
        PendingDirection = AimDirection;
        IsAttacking = true;
        Anim.SetTrigger(HashAttack);
    }

    public override void OnAttackComplete()
    {
        IsAttacking = false;
    }
}