using UnityEngine;

/// <summary>
/// Configuration data for ranged-based enemy characters.
/// Extends EnemyData with projectile-based attack mechanics.
/// </summary>
[CreateAssetMenu(fileName = "RangedEnemyData", menuName = "Scriptable Objects/Enemies/RangedEnemyData")]
public class RangedEnemyData : EnemyData
{
    #region Projectile Configuration

    /// <summary>
    /// Defines the properties and behavior of fired projectiles.
    /// </summary>
    public ProjectileData projectileData;

    #endregion
}
