using UnityEngine;

/// <summary>
/// Assassin-class melee character data.
/// Specialises in evasion, mobility, and burst damage.
/// </summary>
[CreateAssetMenu(fileName = "RogueData", menuName = "Scriptable Objects/Players/RogueData")]
public class RogueData : MeleeData
{
    #region Mobility

    /// <summary>
    /// Movement speed during dash ability.
    /// </summary>
    /// <remarks>TODO: Implement dash mechanic for mobility and repositioning.</remarks>
    public float dashSpeed = 12f;

    /// <summary>
    /// Cooldown time in seconds between consecutive dashes.
    /// </summary>
    /// <remarks>TODO: Implement dash ability cooldown.</remarks>
    public float dashCooldown = 1f;

    #endregion

    #region Defensive

    /// <summary>
    /// Probability of avoiding incoming damage entirely.
    /// Range: 0 to 1 (0% to 100%).
    /// </summary>
    [Range(0f, 1f)]
    public float dodgeChance = 0.1f;

    #endregion

    #region Offensive

    /// <summary>
    /// Damage multiplier for attacks executed from behind the target.
    /// </summary>
    public float backstabMultiplier = 2f;

    #endregion
}
