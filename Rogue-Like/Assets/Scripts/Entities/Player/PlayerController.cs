using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(MovementComponent))]
public class PlayerController : EntityController
{
    public static PlayerController Instance { get; private set; }

    [Header("Data")]
    [SerializeField] protected PlayerData playerData;

    [Header("Input")]
    [SerializeField] protected InputActionAsset inputActions;

    private MovementComponent moveComp;

    protected InputAction moveAction;
    protected Vector2 moveInput;
    protected Vector2 lastMoveDir;

    protected override float MaxHealth => playerData.health;
    protected override float GetDefence() => playerData.defence;

    protected InputActionMap PlayerActionMap { get; private set; }

    protected override void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;

        base.Awake();

        moveComp = GetComponent<MovementComponent>();
        moveComp.Init(Rb, playerData.speed);

        PlayerActionMap = inputActions.FindActionMap("Player");
        moveAction = PlayerActionMap.FindAction("Move");

        if (playerData.sprite != null) Sprite.sprite = playerData.sprite;

        lastMoveDir = Vector2.right;
    }

    protected virtual void OnEnable() => moveAction.Enable();
    protected virtual void OnDisable() => moveAction.Disable();

    protected virtual void Update()
    {
        if (IsDead) return;

        moveInput = moveAction.ReadValue<Vector2>();

        if (moveInput != Vector2.zero) lastMoveDir = moveInput.normalized;

        if (moveInput.x != 0f) Sprite.flipX = moveInput.x < 0f;
    }

    private void FixedUpdate()
    {
        if (IsDead) return;
        moveComp.Move(moveInput);
    }
}