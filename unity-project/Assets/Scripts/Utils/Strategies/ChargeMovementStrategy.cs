using UnityEngine;

public class ChargeMovementStrategy : IMeleeMovementStrategy
{
    private readonly MoveComponent moveComp;
    private readonly float chargeSpeed;
    private readonly float chargeCooldown;
    private readonly float normalSpeed;

    private float nextChargeTime = 0f;
    private bool isCharging = false;
    private float chargeEndTime = 0f;
    private const float ChargeDuration = 0.4f;

    public ChargeMovementStrategy(MoveComponent moveComp, float chargeSpeed, float chargeCooldown)
    {
        this.moveComp = moveComp;
        this.chargeSpeed = chargeSpeed;
        this.chargeCooldown = chargeCooldown;
        normalSpeed = moveComp.GetBaseSpeed();
    }

    public Vector2 GetMoveDirection(Vector2 dirToTarget, float sqrDistToTarget, float circleStrafeDist)
    {
        float now = Time.time;

        if (isCharging)
        {
            if (now >= chargeEndTime)
            {
                isCharging = false;
                moveComp.ResetSpeed();
                nextChargeTime = now + chargeCooldown;
            }
            return dirToTarget;
        }

        if (now >= nextChargeTime)
        {
            isCharging = true;
            chargeEndTime = now + ChargeDuration;
            moveComp.SetSpeed(chargeSpeed);
        }

        return dirToTarget;
    }
}