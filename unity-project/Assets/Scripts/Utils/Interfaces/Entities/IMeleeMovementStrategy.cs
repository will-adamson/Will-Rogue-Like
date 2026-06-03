using UnityEngine;

public interface IMeleeMovementStrategy
{
    Vector2 GetMoveDirection(Vector2 dirToTarget, float sqrDistToTarget, float circleStrafeDist);
}