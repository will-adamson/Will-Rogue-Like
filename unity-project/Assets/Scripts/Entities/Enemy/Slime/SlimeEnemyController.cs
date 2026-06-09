using System.Collections;
using UnityEngine;

public class SlimeEnemyController : MeleeEnemyController
{
    [Header("Slime Split")]
    [SerializeField] private GameObject smallSlimePrefab;
    [SerializeField] private int splitCount = 2;
    [SerializeField] private float splitSpread = 1.2f;

    [Header("Patrol")]
    [SerializeField] private float patrolSpeedMultiplier = 0.5f;
    [SerializeField] private float patrolChangeInterval = 3f;
    [SerializeField] private float patrolChangeIntervalVariance = 1f;

    [Header("Separation")]
    [SerializeField] private float separationRadius = 0.8f;
    [SerializeField] private float separationForce = 2f;

    [Header("Stimulus Detection")]
    [SerializeField] private float stimulusDetectionRadius = 6f;
    [SerializeField] private LayerMask stimulusLayer;

    private Vector2 patrolDirection;
    private float currentPatrolInterval;
    private float patrolChangeTimer = 0f;
    private EnemyState state = EnemyState.Patrol;
    private bool isSplit = false;
    private Transform attractedTo = null;

    protected override void Awake()
    {
        base.Awake();
        PickNewPatrolDirection();
        if (isSplit) StartCoroutine(SpawnScaleIn());
    }

    protected override void Update()
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
        CheckForStimulus();

        if (attractedTo != null)
        {
            Vector2 dirToStimulus =
                ((Vector2)attractedTo.position - (Vector2)transform.position).normalized;
            MovementComp.SetSpeed(Data.speed * patrolSpeedMultiplier * 1.5f);
            MovementComp.Move(dirToStimulus);
            Sprite.flipX = dirToStimulus.x < 0f;
            return;
        }

        patrolChangeTimer += Time.deltaTime;
        if (patrolChangeTimer >= currentPatrolInterval)
            PickNewPatrolDirection();

        MovementComp.SetSpeed(Data.speed * patrolSpeedMultiplier);
        MovementComp.Move(patrolDirection);
        Sprite.flipX = patrolDirection.x < 0f;
    }

    private void PickNewPatrolDirection()
    {
        float angle = Random.Range(0f, 360f) * Mathf.Deg2Rad;
        patrolDirection = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));

        patrolChangeTimer = 0f;
        currentPatrolInterval = patrolChangeInterval + Random.Range(
            -patrolChangeIntervalVariance, patrolChangeIntervalVariance);
    }

    private void CheckForStimulus()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(
            transform.position, stimulusDetectionRadius, stimulusLayer);

        float closest = float.MaxValue;
        attractedTo = null;

        foreach (Collider2D col in hits)
        {
            if (!col.TryGetComponent(out SlimeStimulus stimulus)) continue;

            float dist = Vector2.SqrMagnitude(
                (Vector2)transform.position - (Vector2)col.transform.position);

            if (dist < closest)
            {
                closest = dist;
                attractedTo = col.transform;
            }
        }
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

    public void AlertImmediate()
    {
        state = EnemyState.Chase;
        MovementComp.ResetSpeed();
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

        Vector2 spawnPos = transform.position;

        for (int i = 0; i < splitCount; i++)
        {
            float angle = 360f / splitCount * i * Mathf.Deg2Rad;
            Vector2 offset = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * splitSpread;

            GameObject split = Instantiate(smallSlimePrefab,
                spawnPos + offset, Quaternion.identity);

            split.transform.localScale = transform.localScale * 0.6f;

            if (split.TryGetComponent(out SlimeEnemyController splitController))
            {
                splitController.isSplit = true;
                splitController.AlertImmediate();
            }
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