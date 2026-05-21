using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(ProjectileAttackComponent))]
public class ArcherPlayerController : PlayerController
{
    private static readonly int HashAttack = Animator.StringToHash("Attack");

    private ArcherData archerData;
    private InputAction attackAction;

    protected override void Awake()
    {
        base.Awake();

        archerData = playerData as ArcherData;
        if (archerData == null) return;

        ProjectileAttackComponent projectileAttackComp = GetComponent<ProjectileAttackComponent>();
        projectileAttackComp.Init(archerData.arrowData, bonusDamage: archerData.damage);

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