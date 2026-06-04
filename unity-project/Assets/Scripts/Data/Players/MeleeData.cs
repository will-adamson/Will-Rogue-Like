using UnityEngine;

/// <summary>
/// Base class for melee-based player characters.
/// Extends PlayerData with close-range combat mechanics.
/// </summary>
public abstract class MeleeData : PlayerData
{
    #region Melee Combat

    /// <summary>
    /// Cooldown time in seconds between consecutive melee attacks.
    /// </summary>
    public float attackCooldown = 0.5f;

    /// <summary>
    /// Force applied to enemies on hit, determining knockback distance.
    /// </summary>
    public float knockbackForce = 3f;

    /// <summary>
    /// Maximum distance in units at which melee attacks can connect.
    /// </summary>
    public float attackRange = 0.6f;

    #endregion
}
