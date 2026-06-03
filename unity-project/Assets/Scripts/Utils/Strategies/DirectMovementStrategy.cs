using UnityEngine;

public class DirectMovementStrategy : IMeleeMovementStrategy
{
    public Vector2 GetMoveDirection(Vector2 dirToTarget, float sqrDistToTarget, float circleStrafeDist)
        => dirToTarget;
}