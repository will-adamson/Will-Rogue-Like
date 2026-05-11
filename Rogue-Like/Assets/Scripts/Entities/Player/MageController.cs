using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(ProjectileAttackComponent))]
public class MageController : PlayerController
{
    private ProjectileAttackComponent projectileAttackComp;
    private InputAction projectAction;

    protected override void Awake()
    {
        base.Awake();

        projectileAttackComp = GetComponent<ProjectileAttackComponent>();
        projectileAttackComp.Init(playerData.projectileData, bonusDamage: playerData.damage);
        
        projectAction = PlayerActionMap.FindAction("Project");
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        projectAction.Enable();
        projectAction.performed += OnAttackPerformed;
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        projectAction.Disable();
        projectAction.performed -= OnAttackPerformed;
    }

    private void OnAttackPerformed(InputAction.CallbackContext ctx)
    {
        if (IsDead) return;
        projectileAttackComp.Attack(lastMoveDir);
    }
}