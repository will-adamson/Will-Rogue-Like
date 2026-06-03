using UnityEngine;

/// <summary>
/// Marksman-class ranged character data.
/// Specialises in precise, high-damage single-target attacks with projectiles.
/// </summary>
[CreateAssetMenu(fileName = "ArcherData", menuName = "Scriptable Objects/Players/ArcherData")]
public class ArcherData : PlayerData
{
    #region Projectile Configuration

    /// <summary>
    /// Defines the properties and behavior of fired arrows.
    /// </summary>
    public ProjectileData arrowData;

    #endregion

    #region Attack Mechanics

    /// <summary>
    /// Cooldown time in seconds between arrow shots.
    /// </summary>
    public float drawCooldown = 0.6f;

    /// <summary>
    /// Maximum distance in units at which arrows can travel.
    /// </summary>
    public float arrowRange = 12f;

    /// <summary>
    /// Movement speed of arrows in units per second.
    /// </summary>
    public float arrowSpeed = 15f;

    #endregion

    #region Projectile Behavior

    /// <summary>
    /// Number of arrows fired per shot.
    /// </summary>
    /// <remarks>TODO: Implement multi-arrow shot mechanic for advanced archery.</remarks>
    public int arrowsPerShot = 1;

    /// <summary>
    /// Angle in degrees between multiple arrows for spread pattern.
    /// </summary>
    public float spreadAngle = 10f;

    #endregion

    #region Special Abilities

    /// <summary>
    /// Damage multiplier applied to charged shots.
    /// </summary>
    /// <remarks>TODO: Implement charged shot mechanic for increased damage output.</remarks>
    public float chargedShotMultiplier = 2f;

    #endregion
}
