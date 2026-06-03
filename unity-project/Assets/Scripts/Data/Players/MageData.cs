using UnityEngine;

/// <summary>
/// Ranged spellcaster character data.
/// Specialises in area-of-effect damage and crowd control through projectile magic.
/// </summary>
[CreateAssetMenu(fileName = "MageData", menuName = "Scriptable Objects/Players/MageData")]
public class MageData : PlayerData
{
    #region Projectile Configuration

    /// <summary>
    /// Defines the properties and behavior of cast projectiles.
    /// </summary>
    public ProjectileData projectileData;

    #endregion

    #region Casting Mechanics

    /// <summary>
    /// Cooldown time in seconds between spell casts.
    /// </summary>
    public float castCooldown = 0.3f;

    /// <summary>
    /// Maximum distance in units at which spells can be cast.
    /// </summary>
    public float spellRange = 8f;

    /// <summary>
    /// Movement speed of spell projectiles in units per second.
    /// </summary>
    public float spellSpeed = 10f;

    #endregion

    #region Projectile Behavior

    /// <summary>
    /// Number of projectiles spawned per spell cast.
    /// </summary>
    public int projectilesPerCast = 1;

    /// <summary>
    /// Angle in degrees between multiple projectiles for spread pattern.
    /// </summary>
    public float spreadAngle = 15f;

    #endregion
}
