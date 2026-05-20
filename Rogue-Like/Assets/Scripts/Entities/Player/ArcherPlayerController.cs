using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(ProjectileAttackComponent))]
public class ArcherPlayerController : PlayerController
{
    private static readonly int HashAttack = Animator.StringToHash("Attack");
    private static readonly int HashIsWalking = Animator.StringToHash("IsWalking");

    private ArcherData archerData;
    private IAttacker attacker;
    private InputAction attackAction;
    private Vector2 pendingDirection;

    protected override void Awake()
    {
        base.Awake();

        archerData = playerData as ArcherData;

        if (archerData == null) return;

        ProjectileAttackComponent projectileAttackComp = GetComponent<ProjectileAttackComponent>();
        projectileAttackComp.Init(archerData.arrowData, bonusDamage: archerData.damage);

        attacker = projectileAttackComp;
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

    public void FireArrow()
    {
        attacker.Attack(pendingDirection);
    }
}