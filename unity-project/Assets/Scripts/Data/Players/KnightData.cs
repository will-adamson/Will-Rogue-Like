using UnityEngine;

/// <summary>
/// Tank-class melee character data.
/// Specialises in defense and crowd control with heavy attacks.
/// </summary>
[CreateAssetMenu(fileName = "KnightData", menuName = "Scriptable Objects/Players/KnightData")]
public class KnightData : MeleeData
{
    #region Knight Abilities

    /// <summary>
    /// Damage reduction percentage when blocking.
    /// Range: 0 to 1 (0% to 100%).
    /// </summary>
    /// <remarks>TODO: Implement blocking mechanic in combat system.</remarks>
    public float blockDamageReduction = 0.5f;

    /// <summary>
    /// Movement speed multiplier during charge attack.
    /// </summary>
    /// <remarks>TODO: Implement charge attack ability.</remarks>
    public float chargeSpeed = 8f;

    #endregion
}
