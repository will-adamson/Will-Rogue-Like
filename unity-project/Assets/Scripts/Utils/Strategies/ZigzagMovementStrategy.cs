using UnityEngine;

public class ZigzagMovementStrategy : IMeleeMovementStrategy
{
    private readonly float amplitude;
    private readonly float frequency;

    public ZigzagMovementStrategy(float amplitude, float frequency)
    {
        this.amplitude = amplitude;
        this.frequency = frequency;
    }

    public Vector2 GetMoveDirection(Vector2 dirToTarget, float sqrDistToTarget, float circleStrafeDist)
    {
        Vector2 perp = new Vector2(-dirToTarget.y, dirToTarget.x);
        float offset = Mathf.Sin(Time.time * frequency) * amplitude;
        return (dirToTarget + perp * offset).normalized;
    }
}