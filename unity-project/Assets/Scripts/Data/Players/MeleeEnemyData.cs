using UnityEngine;

[CreateAssetMenu(fileName = "MeleeEnemyData", menuName = "Scriptable Objects/Enemies/MeleeEnemyData")]
public class MeleeEnemyData : EnemyData
{
    public float damage = 10f;
    public float knockbackForce = 2f;
    public float attackCooldown = 1f;

    /// <summary>Point = single target, Arc = cone, Radial = omnidirectional.</summary>
    public AttackShape attackShape = AttackShape.Point;

    /// <summary>Cone width in degrees. Arc shape only.</summary>
    public float attackAngle = 45f;

    /// <summary>Max targets hit simultaneously. Radial shape only.</summary>
    public int attackHitCount = 1;

    public MovementPattern movementPattern = MovementPattern.Direct;

    /// <summary>Ideal distance from target. CircleStrafe pattern only.</summary>
    public float circleStrafeDist = 2f;

    /// <summary>Movement speed during charge. Charge pattern only.</summary>
    public float chargeSpeed = 8f;

    /// <summary>Seconds between charges. Charge pattern only.</summary>
    public float chargeCooldown = 3f;

    /// <summary>Lateral offset amplitude. Zigzag pattern only.</summary>
    public float zigzagAmplitude = 1.5f;

    /// <summary>Direction-change frequency. Higher = more frequent. Zigzag pattern only.</summary>
    public float zigzagFrequency = 3f;

    public bool canBeStaggered = true;
    public float staggerThreshold = 0f;

    /// <summary>Status effect applied on hit (Slow, Poison, etc.). Null for none.</summary>
    public StatusEffect onHitEffect;

    /// <summary>Chance to spawn smaller enemies on death. Range: 0–1.</summary>
    public float splitOnDeathChance = 0f;
}
