using UnityEngine;

public class PlayerDetectorComponent : MonoBehaviour, IDetector
{
    private float detectionRangeSqrt;
    private float attackRangeSqrt;

    public void Init(float detectionRange, float attackRange)
    {
        detectionRangeSqrt = detectionRange * detectionRange;
        attackRangeSqrt = attackRange * attackRange;
    }

    public bool IsTargetDetected(out Vector2 directionToTarget, out float sqrtDistance)
    {
        directionToTarget = Vector2.zero;
        sqrtDistance = float.MaxValue;

        if (PlayerController.Instance == null) return false;

        Vector2 toPlayer = (Vector2)(PlayerController.Instance.transform.position - transform.position);
        sqrtDistance = toPlayer.sqrMagnitude;

        if (sqrtDistance > detectionRangeSqrt) return false;

        directionToTarget = toPlayer.normalized;

        return true;
    }

    public bool IsInAttackRange(float sqrtDistance) => sqrtDistance <= attackRangeSqrt;
}