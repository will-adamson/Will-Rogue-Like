using UnityEngine;

/// <summary>
/// Defines the properties and behavior of projectile objects.
/// Used by ranged characters and ranged enemies for attack configuration.
/// </summary>
[CreateAssetMenu(fileName = "ProjectileData", menuName = "Scriptable Objects/ProjectileData")]
public class ProjectileData : ScriptableObject
{
    #region Identity

    /// <summary>
    /// Display name of the projectile type.
    /// </summary>
    public new string name;

    /// <summary>
    /// Prefab to instantiate when projectile is fired.
    /// </summary>
    public GameObject projectilePrefab;

    #endregion

    #region Combat Stats

    /// <summary>
    /// Damage dealt to targets on impact.
    /// </summary>
    public float damage = 10f;

    /// <summary>
    /// Movement speed in units per second.
    /// </summary>
    public float speed = 10f;

    /// <summary>
    /// Maximum travel distance in units before projectile despawns.
    /// </summary>
    public float range = 5f;

    #endregion

    #region Attack Mechanics

    /// <summary>
    /// Cooldown time in seconds between projectile launches.
    /// </summary>
    public float cooldown = 0.3f;

    #endregion

    #region Object Pool

    /// <summary>
    /// Number of projectile instances to pre-allocate for object pooling.
    /// Improves performance by reducing instantiation overhead.
    /// </summary>
    public int poolSize = 10;

    #endregion
}
