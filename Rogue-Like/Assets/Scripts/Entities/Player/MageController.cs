using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(ProjectileAttackComponent))]
public class MageController : PlayerController
{
    private IAttacker attacker;
    private InputAction attackAction;

    protected override void Awake()
    {
        base.Awake();

        var projectileAttackComp = GetComponent<ProjectileAttackComponent>();
        projectileAttackComp.Init(playerData.projectileData, bonusDamage: playerData.damage);

        attacker = projectileAttackComp;
        attackAction = PlayerActionMap.FindAction("Project");
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        attackAction.Enable();
        attackAction.performed += OnAttackPerformed;
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        attackAction.Disable();
        attackAction.performed -= OnAttackPerformed;
    }

    private void OnAttackPerformed(InputAction.CallbackContext ctx)
    {
        if (IsDead) return;
        attacker.Attack(AimDirection);
    }
}