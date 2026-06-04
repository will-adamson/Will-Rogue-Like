using UnityEngine;

/// <summary>
/// Abstract base class for enemy character data.
/// Defines core attributes and detection mechanics shared across all enemy types.
/// </summary>
public abstract class EnemyData : ScriptableObject
{
    #region Identity

    /// <summary>
    /// Visual representation of the enemy character.
    /// </summary>
    public Sprite sprite;

    /// <summary>
    /// Display name of the enemy type.
    /// </summary>
    public new string name;

    #endregion

    #region Base Stats

    /// <summary>
    /// Maximum health points. Determines how much damage the enemy can take.
    /// </summary>
    public float health = 100f;

    /// <summary>
    /// Movement speed in units per second.
    /// </summary>
    public float speed = 2f;

    /// <summary>
    /// Damage reduction from incoming attacks. Reduces damage taken by this amount.
    /// </summary>
    public float defence = 0f;

    #endregion

    #region Detection

    /// <summary>
    /// Distance in units at which the enemy detects and targets the player.
    /// </summary>
    public float detectionRange = 6f;

    /// <summary>
    /// Distance in units at which the enemy can execute attacks.
    /// </summary>
    public float attackRange = 1f;

    #endregion

    #region Behavior

    /// <summary>
    /// If true, enemy remains stationary and does not patrol or chase.
    /// </summary>
    public bool isHoldPosition = false;

    /// <summary>
    /// Name shown in the HUD for the enemy's incoming attack.
    /// </summary>
    public string attackName = "Attack";

    /// <summary>
    /// Duration in seconds that the telegraph panel stays visible.
    /// </summary>
    public float telegraphDuration = 2f;

    /// <summary>
    /// Threat level styling applied to the telegraph panel.
    /// </summary>
    public TelegraphDanger danger = TelegraphDanger.Low;

    #endregion

    #region Progression

    /// <summary>
    /// Experience points awarded to player upon defeating this enemy.
    /// </summary>
    public int xpReward = 10;

    #endregion
}
