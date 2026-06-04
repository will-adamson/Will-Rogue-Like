using UnityEngine;

public class PlayerDetectorComponent : MonoBehaviour, IDetector
{
    private float detectionRangeSqr;
    private float attackRangeSqr;

    public void Init(float detectionRange, float attackRange)
    {
        detectionRangeSqr = detectionRange * detectionRange;
        attackRangeSqr = attackRange * attackRange;
    }

    public bool IsTargetDetected(out Vector2 directionToTarget, out float sqrDistance)
    {
        directionToTarget = Vector2.zero;
        sqrDistance = float.MaxValue;

        if (PlayerController.Instance == null) return false;

        Vector2 toPlayer = (Vector2)(PlayerController.Instance.transform.position - transform.position);
        sqrDistance = toPlayer.sqrMagnitude;

        if (sqrDistance > detectionRangeSqr) return false;

        directionToTarget = toPlayer.normalized;
        return true;
    }

    public bool IsInAttackRange(float sqrDistance) => sqrDistance <= attackRangeSqr;
}