using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(ProjectileAttackComponent))]
public class MagePlayerController : PlayerController
{
    private static readonly int HashAttack = Animator.StringToHash("Attack");
    private static readonly int HashIsWalking = Animator.StringToHash("IsWalking");

    private MageData mageData;
    private IAttacker attacker;
    private InputAction attackAction;
    private Vector2 pendingDirection;

    protected override void Awake()
    {
        base.Awake();

        mageData = playerData as MageData;

        if (mageData == null) return;

        ProjectileAttackComponent projectileAttackComp = GetComponent<ProjectileAttackComponent>();
        projectileAttackComp.Init(mageData.projectileData, bonusDamage: mageData.damage);

        attacker = projectileAttackComp;
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

    protected override void Update()
    {
        base.Update();
        Anim.SetBool(HashIsWalking, moveInput != Vector2.zero);
    }

    private void OnAttackPerformed(InputAction.CallbackContext ctx)
    {
        if (IsDead) return;
        pendingDirection = AimDirection;
        Anim.SetTrigger(HashAttack);
    }

    public void FireProjectile()
    {
        attacker.Attack(pendingDirection);
    }
}