using UnityEngine;

public class CircleStrafeMovementStrategy : IMeleeMovementStrategy
{
    private float strafeSign = 1f;
    private float changeTimer = 0f;
    private const float ChangeInterval = 2f;

    public Vector2 GetMoveDirection(Vector2 dirToTarget, float sqrDistToTarget, float circleStrafeDist)
    {
        changeTimer += Time.deltaTime;
        if (changeTimer >= ChangeInterval)
        {
            strafeSign *= -1f;
            changeTimer = 0f;
        }

        float preferredSqr = circleStrafeDist * circleStrafeDist;

        Vector2 perp = new Vector2(-dirToTarget.y, dirToTarget.x) * strafeSign;

        float radial = sqrDistToTarget > preferredSqr ? 1f : -1f;

        return (perp + dirToTarget * radial).normalized;
    }
}