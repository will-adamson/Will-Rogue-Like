using UnityEngine;

/// <summary>
/// Defines temporary status conditions applied to characters.
/// Examples: Slow, Poison, Burn, Stun. Applied via attacks or environmental effects.
/// </summary>
[CreateAssetMenu(fileName = "StatusEffect", menuName = "Scriptable Objects/StatusEffect")]
public class StatusEffect : ScriptableObject
{
    #region Identity

    /// <summary>
    /// Display name of the status effect.
    /// </summary>
    public string effectName;

    #endregion

    #region Duration

    /// <summary>
    /// How long the effect persists in seconds.
    /// </summary>
    public float duration = 2f;

    #endregion

    #region Stat Modifiers

    /// <summary>
    /// Multiplier applied to movement speed while effect is active.
    /// Range: 0 to 2+ (0% to 200%+). Example: 0.5 = 50% slow.
    /// </summary>
    public float speedMultiplier = 1f;

    /// <summary>
    /// Damage per second dealt while effect is active.
    /// Used for damage-over-time effects like poison or burn.
    /// </summary>
    public float damagePerSecond = 0f;

    #endregion
}
