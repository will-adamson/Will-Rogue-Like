using UnityEngine;
using UnityEngine.InputSystem;

public class WarriorController : PlayerController
{
    private InputAction attackAction;

    private static readonly int HashAttack = Animator.StringToHash("isAttacking");

    protected override void Awake()
    {
        base.Awake();

        attackAction = PlayerActionMap.FindAction("Attack");
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
        Animator.SetTrigger(HashAttack);
    }
}