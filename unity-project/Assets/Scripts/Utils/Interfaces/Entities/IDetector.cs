using UnityEngine;

public interface IDetector
{
    bool IsTargetDetected(out Vector2 directionToTarget, out float sqrtDistance);
}