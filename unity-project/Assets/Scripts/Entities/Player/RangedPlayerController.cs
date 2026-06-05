using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(ProjectileAttackComponent))]
public class RangedPlayerController : PlayerController
{
    private static readonly int HashAttack = Animator.StringToHash("Attack");

    private RangedPlayerData rangedPlayerData;
    private InputAction attackAction;

    protected override void Awake()
    {
        base.Awake();

        rangedPlayerData = playerData as RangedPlayerData;
        if (rangedPlayerData == null) return;

        ProjectileAttackComponent projectileAttackComp = GetComponent<ProjectileAttackComponent>();
        projectileAttackComp.Init(rangedPlayerData.arrowData, bonusDamage: rangedPlayerData.damage);

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
        Anim.SetTrigger(HashAttack);
    }
}