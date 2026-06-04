using UnityEngine;

/// <summary>
/// Abstract base class for player character data.
/// Defines core attributes and progression mechanics shared across all player classes.
/// </summary>
public abstract class PlayerData : ScriptableObject
{
    #region Identity

    /// <summary>
    /// Visual representation of the player character.
    /// </summary>
    public Sprite sprite;

    /// <summary>
    /// Display name of the player class.
    /// </summary>
    public new string name;

    /// <summary>
    /// Classification of the player's combat style.
    /// </summary>
    public PlayerType playerType;

    #endregion

    #region Base Stats

    /// <summary>
    /// Maximum health points. Determines how much damage the player can take.
    /// </summary>
    public float health = 100f;

    /// <summary>
    /// Maximum stamina points. Used for class-specific abilities.
    /// </summary>
    public float stamina = 100f;

    /// <summary>
    /// Base damage output per attack before modifiers.
    /// </summary>
    public float damage = 10f;

    /// <summary>
    /// Movement speed in units per second.
    /// </summary>
    public float speed = 5f;

    /// <summary>
    /// Damage reduction from incoming attacks. Reduces damage taken by this amount.
    /// </summary>
    public float defence = 0f;

    #endregion

    #region Critical Damage

    /// <summary>
    /// Probability of landing a critical strike. Range: 0 to 1 (0% to 100%).
    /// </summary>
    [Range(0f, 1f)]
    public float critChance = 0f;

    /// <summary>
    /// Damage multiplier applied to critical strikes.
    /// </summary>
    public float critMultiplier = 1.5f;

    #endregion

    #region Progression

    /// <summary>
    /// Experience points required to advance to the next level.
    /// </summary>
    public float experienceToNextLevel = 100f;

    #endregion
}
