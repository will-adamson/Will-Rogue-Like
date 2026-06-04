using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(MeleeAttackComponent))]
public class MeleePlayerController : PlayerController
{
    private const string LAYER_NAME_ENEMY = "Enemy";

    private InputAction attackAction;
    private MeleeAttackComponent meleeAttackComp;

    private static readonly int HashAttack = Animator.StringToHash("Attack");

    protected override void Awake()
    {
        base.Awake();

        MeleeData meleeData = playerData as MeleeData;

        attackAction = PlayerActionMap.FindAction("Attack");

        meleeAttackComp = GetComponent<MeleeAttackComponent>();
        meleeAttackComp.Init(playerData.damage, meleeData.knockbackForce, meleeData.attackCooldown, LAYER_NAME_ENEMY);
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

        if (meleeAttackComp != null && meleeAttackComp.Attack(AimDirection))
            Anim.SetTrigger(HashAttack);
    }
}