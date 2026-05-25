using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(MoveComponent))]
public class PlayerController : EntityController, IAimProvider
{
    public static PlayerController Instance { get; private set; }

    [Header("SO Data")]
    [SerializeField] protected PlayerData playerData;

    [Header("Input")]
    [SerializeField] protected InputActionAsset inputActions;

    [Header("Death")]
    [SerializeField] private GameObject[] tombstonePrefabs;

    private MoveComponent moveComp;
    private InputAction moveAction;

    protected Vector2 moveInput;

    protected IAttacker Attacker { get; set; }
    protected Vector2 PendingDirection { get; set; }

    public Vector2 AimDirection { get; private set; } = Vector2.right;

    protected override float MaxHealth => playerData.health;
    protected override float GetDefence() => playerData.defence;

    protected InputActionMap PlayerActionMap { get; private set; }

    private static readonly int HashIsWalking = Animator.StringToHash("IsWalking");

    protected override void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;

        base.Awake();

        moveComp = GetComponent<MoveComponent>();
        moveComp.Init(Rb, playerData.speed);

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

        Anim.SetBool(HashIsWalking, moveInput != Vector2.zero);
    }

    private void FixedUpdate()
    {
        if (IsDead) return;
        moveComp.Move(moveInput);
    }

    public void FireAttack() => Attacker?.Attack(PendingDirection);

    protected override void HandleDeath()
    {
        if (tombstonePrefabs != null && tombstonePrefabs.Length > 0)
        {
            GameObject randomTombstone = tombstonePrefabs[Random.Range(0, tombstonePrefabs.Length)];
            Instantiate(randomTombstone, transform.position, Quaternion.identity);
        }

        Destroy(gameObject);
    }
}