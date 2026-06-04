using System.Collections;
using UnityEngine;

public class SlimeEnemyController : MeleeEnemyController
{
    [Header("Slime Split")]
    [SerializeField] private GameObject smallSlimePrefab;
    [SerializeField] private int splitCount = 2;
    [SerializeField] private float splitSpread = 1.2f;

    [Header("Patrol")]
    [SerializeField] private Vector2 patrolDirection = Vector2.right;
    [SerializeField] private float patrolSpeedMultiplier = 0.5f;
    [SerializeField] private float patrolChangeInterval = 3f;

    [Header("Separation")]
    [SerializeField] private float separationRadius = 0.8f;
    [SerializeField] private float separationForce = 2f;

    private EnemyState state = EnemyState.Patrol;
    private bool isSplit = false;
    private float patrolChangeTimer = 0f;

    protected override void Awake()
    {
        base.Awake();
        if (isSplit) StartCoroutine(SpawnScaleIn());
    }

    private void Update()
    {
        if (IsDead) return;

        bool playerDetected = PlayerDetectorComp.IsTargetDetected(
            out Vector2 dirToPlayer, out float sqrDist);

        switch (state)
        {
            case EnemyState.Patrol:
                if (playerDetected)
                {
                    TransitionToAlert();
                    return;
                }
                HandlePatrol();
                break;

            case EnemyState.Alert:
                break;

            case EnemyState.Chase:
            case EnemyState.Attack:
                if (!playerDetected)
                {
                    state = EnemyState.Patrol;
                    MovementComp.ResetSpeed();
                    return;
                }

                Sprite.flipX = dirToPlayer.x < 0f;
                bool inAttackRange = PlayerDetectorComp.IsInAttackRange(sqrDist);

                if (!inAttackRange && !Data.isHoldPosition)
                    HandleMovement(dirToPlayer, sqrDist);
                else
                    MovementComp.Move(Vector2.zero);

                if (inAttackRange)
                {
                    state = EnemyState.Attack;
                    HandleAttack(dirToPlayer);
                }
                else
                {
                    state = EnemyState.Chase;
                }
                ApplySeparation();
                break;
        }
    }

    private void HandlePatrol()
    {
        patrolChangeTimer += Time.deltaTime;
        if (patrolChangeTimer >= patrolChangeInterval)
        {
            patrolDirection *= -1f;
            patrolChangeTimer = 0f;
        }

        MovementComp.SetSpeed(Data.speed * patrolSpeedMultiplier);
        MovementComp.Move(patrolDirection);
        Sprite.flipX = patrolDirection.x < 0f;
    }

    private void TransitionToAlert()
    {
        state = EnemyState.Alert;
        MovementComp.Move(Vector2.zero);
        StartCoroutine(AlertRoutine());
    }

    private IEnumerator AlertRoutine()
    {
        yield return new WaitForSeconds(0.5f);
        MovementComp.ResetSpeed();
        state = EnemyState.Chase;
    }

    private void ApplySeparation()
    {
        Collider2D[] neighbours = Physics2D.OverlapCircleAll(
            transform.position, separationRadius, LayerMask.GetMask("Enemy"));

        foreach (Collider2D col in neighbours)
        {
            if (col.gameObject == gameObject) continue;

            Vector2 away = (Vector2)(transform.position - col.transform.position);
            float strength = 1f - (away.magnitude / separationRadius);
            Rb.AddForce(separationForce * strength * away.normalized, ForceMode2D.Force);
        }
    }

    protected override void HandleDeath()
    {
        MeleeEnemyData data = Data as MeleeEnemyData;
        if (!isSplit && data != null && Random.value <= data.splitOnDeathChance)
            StartCoroutine(SpawnSplitsNextFrame());
        base.HandleDeath();
    }

    private IEnumerator SpawnSplitsNextFrame()
    {
        yield return null;

        for (int i = 0; i < splitCount; i++)
        {
            float angle = 360f / splitCount * i * Mathf.Deg2Rad;
            Vector2 offset = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * splitSpread;

            GameObject split = Instantiate(smallSlimePrefab,
                (Vector2)transform.position + offset, Quaternion.identity);

            split.transform.localScale = transform.localScale * 0.6f;

            if (split.TryGetComponent(out DamageFlashComponent flash))
                flash.ResetColor();

            if (split.TryGetComponent(out SlimeEnemyController splitController))
                splitController.isSplit = true;
        }
    }

    private IEnumerator SpawnScaleIn()
    {
        Vector3 targetScale = transform.localScale;
        transform.localScale = Vector3.zero;
        float t = 0f;
        float duration = 0.2f;

        while (t < duration)
        {
            t += Time.deltaTime;
            transform.localScale = Vector3.Lerp(Vector3.zero, targetScale, t / duration);
            yield return null;
        }

        transform.localScale = targetScale;
    }
}