using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(ProjectileAttackComponent))]
public class MagePlayerController : PlayerController
{
    private MagePlayerData magePlayerData;
    private InputAction attackAction;

    protected override void Awake()
    {
        base.Awake();

        magePlayerData = playerData as MagePlayerData;
        if (magePlayerData == null) return;

        ProjectileAttackComponent projectileAttackComp = GetComponent<ProjectileAttackComponent>();
        projectileAttackComp.Init(magePlayerData.projectileData, bonusDamage: magePlayerData.damage);

        Attacker = projectileAttackComp;
        attackAction = PlayerActionMap.FindAction("Project");
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