using UnityEngine;

[CreateAssetMenu(fileName = "RogueData", menuName = "Scriptable Objects/Players/RogueData")]
public class RogueData : MeleeData
{
    /// <summary>TODO: implement dash mechanic.</summary>
    public float dashSpeed = 12f;
    /// <summary>TODO: implement dash cooldown.</summary>
    public float dashCooldown = 1f;

    /// <summary>Chance to avoid damage entirely. Range: 0–1.</summary>
    [Range(0f, 1f)]
    public float dodgeChance = 0.1f;

    public float backstabMultiplier = 2f;
}
