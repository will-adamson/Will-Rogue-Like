public static class MovementStrategyFactory
{
    public static IMeleeMovementStrategy Create(MeleeEnemyData data, MoveComponent moveComp)
    {
        return data.movementPattern switch
        {
            MovementPattern.CircleStrafe => new CircleStrafeMovementStrategy(),
            MovementPattern.Charge => new ChargeMovementStrategy(moveComp, data.chargeSpeed, data.chargeCooldown),
            MovementPattern.Zigzag => new ZigzagMovementStrategy(data.zigzagAmplitude, data.zigzagFrequency),
            _ => new DirectMovementStrategy(),
        };
    }
}