using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(MovementComponent), typeof(Animator))]
public class PlayerController : EntityController, IAimProvider
{
    public static PlayerController Instance { get; private set; }

    [Header("Data")]
    [SerializeField] protected PlayerData playerData;

    [Header("Input")]
    [SerializeField] protected InputActionAsset inputActions;

    private MovementComponent moveComp;
    private InputAction moveAction;
    private Vector2 moveInput;

    public Vector2 AimDirection { get; private set; } = Vector2.right;

    protected override float MaxHealth => playerData.health;
    protected override float GetDefence() => playerData.defence;

    protected InputActionMap PlayerActionMap { get; private set; }
    protected Animator Animator { get; private set; }

    protected override void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;

        base.Awake();

        moveComp = GetComponent<MovementComponent>();
        moveComp.Init(Rb, playerData.speed);

        Animator = GetComponent<Animator>();

        PlayerActionMap = inputActions.FindActionMap("Player");
        moveAction = PlayerActionMap.FindAction("Move");

        if (playerData.sprite != null) Sprite.sprite = playerData.sprite;
    }

    protected virtual void OnEnable() => moveAction.Enable();
    protected virtual void OnDisable() => moveAction.Disable();

    protected virtual void Update()
    {
        if (IsDead) return;

        moveInput = moveAction.ReadValue<Vector2>();

        if (moveInput != Vector2.zero) AimDirection = moveInput.normalized;
        if (moveInput.x != 0f) Sprite.flipX = moveInput.x < 0f;
    }

    private void FixedUpdate()
    {
        if (IsDead) return;
        moveComp.Move(moveInput);
    }

    protected override void HandleDeath()
    {
        moveComp.Move(Vector2.zero);
    }
}