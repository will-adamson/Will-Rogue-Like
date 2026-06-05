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
    public float Defence => playerData.defence;

    protected InputActionMap PlayerActionMap { get; private set; }

    private static readonly int HashIsWalking = Animator.StringToHash("IsWalking");
    private static readonly int HashLastDirX = Animator.StringToHash("LastDirX");
    private static readonly int HashLastDirY = Animator.StringToHash("LastDirY");

    private float lastHorizontalDir = 1f; 

    public PlayerStats Stats { get; private set; }

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

        PlayerData data = GameSession.SelectedClass != null ? GameSession.SelectedClass : playerData;
        Stats = new PlayerStats(data);
    }

    protected virtual void OnEnable()
    {
        moveAction.Enable();
        if (Stats != null) Stats.OnChanged += RefreshHUD;
    }

    protected virtual void OnDisable()
    {
        moveAction.Disable();
        if (Stats != null) Stats.OnChanged -= RefreshHUD;
        if (HUDController.Instance != null)
            HUDController.Instance.OnHUDReady -= OnHUDReady;
    }

    protected virtual void Start()
    {
        if (HUDController.Instance == null) return;

        if (HUDController.Instance.IsReady)
        {
            OnHUDReady();
        }
        else
        {
            HUDController.Instance.OnHUDReady += OnHUDReady;
        }
    }

    private void OnHUDReady()
    {
        HUDController.Instance.OnHUDReady -= OnHUDReady;

        PlayerData data = GameSession.SelectedClass != null ? GameSession.SelectedClass : playerData;
        HUDController.Instance.HealthBars.SetCharacter(data);
        RefreshHUD();
        HUDController.Instance.LogFeed.LogSystem("Welcome to Placeholder.");
    }

    protected virtual void Update()
    {
        if (IsDead) return;

        moveInput = moveAction.ReadValue<Vector2>();

        if (moveInput.magnitude > 0.1f)
        {
            Vector2 dir;
            if (Mathf.Abs(moveInput.x) >= Mathf.Abs(moveInput.y))
            {
                dir = new Vector2(Mathf.Sign(moveInput.x), 0);
                lastHorizontalDir = Mathf.Sign(moveInput.x); 
            }
            else
                dir = new Vector2(0, Mathf.Sign(moveInput.y));

            AimDirection = dir;
        }

        Vector2 blendDir = AimDirection;
        if (AimDirection.y < 0 && AimDirection.x == 0)
            blendDir = new Vector2(lastHorizontalDir, 0);

        Anim.SetBool(HashIsWalking, moveInput.magnitude > 0.1f);
        Anim.SetFloat(HashLastDirX, blendDir.x);
        Anim.SetFloat(HashLastDirY, blendDir.y);
    }

    private void FixedUpdate()
    {
        if (IsDead) return;
        moveComp.Move(moveInput);
    }

    public void FireAttack() => Attacker?.Attack(PendingDirection);

    private void RefreshHUD()
    {
        if (HUDController.Instance == null || Stats == null) return;
        HUDController.Instance.HealthBars.Refresh(Stats);
    }

    public void UseStamina(float amount)
    {
        Stats?.ModifyStamina(-amount);
    }

    public void GainExp(float amount)
    {
        if (Stats == null) return;
        int levelBefore = Stats.Level;
        Stats.AddExp(amount);
        HUDController.Instance.LogFeed.LogGold($"You gained {Mathf.RoundToInt(amount)} experience.");
        if (Stats.Level > levelBefore)
            HUDController.Instance.LogFeed.LogSystem($"You reached level {Stats.Level}.");
    }

    protected override void HandleDeath()
    {
        HUDController.Instance.LogFeed.LogSystem("You have died. Game Over.");

        if (Stats != null)
        {
            HUDController.Instance.HealthBars.SetHp(0, Stats.MaxHp);
            HUDController.Instance.HealthBars.SetStamina(Stats.CurrentSta, Stats.MaxSta);
            HUDController.Instance.HealthBars.SetExp(Stats.CurrentExp, Stats.MaxExp, Stats.Level);
        }

        if (tombstonePrefabs != null && tombstonePrefabs.Length > 0)
        {
            GameObject tombstone = tombstonePrefabs[Random.Range(0, tombstonePrefabs.Length)];
            Instantiate(tombstone, transform.position, Quaternion.identity);
        }

        Destroy(gameObject);
    }

    protected override void OnDamageTaken(float rawAmount)
    {
        if (Stats == null) return;
        float actualDamage = Mathf.Max(0f, rawAmount - GetDefence());
        Stats.ModifyHp(-actualDamage);
        HUDController.Instance.LogFeed.LogDamage($"You took {Mathf.RoundToInt(actualDamage)} damage.");
    }
}