using UnityEngine;

/// <summary>
/// Configuration data for melee-based enemy characters.
/// Extends EnemyData with close-range combat mechanics and advanced AI behaviors.
/// </summary>
[CreateAssetMenu(fileName = "MeleeEnemyData", menuName = "Scriptable Objects/Enemies/MeleeEnemyData")]
public class MeleeEnemyData : EnemyData
{
    #region Combat Stats

    /// <summary>
    /// Damage dealt per successful melee attack.
    /// </summary>
    public float damage = 10f;

    /// <summary>
    /// Force applied to targets on hit, determining knockback distance.
    /// </summary>
    public float knockbackForce = 2f;

    /// <summary>
    /// Cooldown time in seconds between consecutive attacks.
    /// </summary>
    public float attackCooldown = 1f;

    #endregion

    #region Attack Shape

    /// <summary>
    /// Determines the attack pattern: Point (single target), Arc (cone), or Radial (omni-directional).
    /// </summary>
    public AttackShape attackShape = AttackShape.Point;

    /// <summary>
    /// Angle in degrees for Arc-shaped attacks (width of cone).
    /// Ignored for Point and Radial shapes.
    /// </summary>
    public float attackAngle = 45f;

    /// <summary>
    /// Maximum number of targets hit by Radial attacks.
    /// Ignored for Point and Arc shapes.
    /// </summary>
    public int attackHitCount = 1;

    #endregion

    #region Movement Behavior

    /// <summary>
    /// Defines how the enemy moves when not in combat.
    /// </summary>
    public MovementPattern movementPattern = MovementPattern.Direct;

    /// <summary>
    /// Optimal distance to maintain from target during CircleStrafe pattern.
    /// Ignored for other movement patterns.
    /// </summary>
    public float circleStrafeDist = 2f;

    /// <summary>
    /// Movement speed during Charge movement pattern.
    /// Ignored for other movement patterns.
    /// </summary>
    public float chargeSpeed = 8f;

    /// <summary>
    /// Cooldown time in seconds between charge attacks.
    /// Ignored for other movement patterns.
    /// </summary>
    public float chargeCooldown = 3f;

    /// <summary>
    /// Lateral movement offset amplitude for Zigzag pattern.
    /// Ignored for other movement patterns.
    /// </summary>
    public float zigzagAmplitude = 1.5f;

    /// <summary>
    /// Oscillation frequency for Zigzag movement pattern.
    /// Higher values = more frequent direction changes.
    /// Ignored for other movement patterns.
    /// </summary>
    public float zigzagFrequency = 3f;

    #endregion

    #region Damage Reaction

    /// <summary>
    /// If true, enemy can be temporarily stunned when hit.
    /// </summary>
    public bool canBeStaggered = true;

    /// <summary>
    /// Damage threshold required to trigger stagger effect.
    /// </summary>
    public float staggerThreshold = 0f;

    #endregion

    #region Special Effects

    /// <summary>
    /// Status effect applied to targets hit by this enemy's attacks.
    /// Examples: Slow, Poison, Burn. Null if no effect.
    /// </summary>
    public StatusEffect onHitEffect;

    /// <summary>
    /// Probability of spawning smaller enemies upon death.
    /// Range: 0 to 1 (0% to 100%). Used by enemies like slimes.
    /// </summary>
    public float splitOnDeathChance = 0f;

    #endregion
}
