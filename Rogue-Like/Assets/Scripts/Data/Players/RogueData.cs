using UnityEngine;

[CreateAssetMenu(fileName = "RogueData", menuName = "Scriptable Objects/Players/RogueData")]
public class RogueData : MeleeData
{
    [Header("Rogue Stats")]
    public float dashSpeed = 12f; // TODO: For future dash mechanic
    public float dashCooldown = 1f;
    [Range(0f, 1f)] public float dodgeChance = 0.1f; // Chance to avoid damage
    public float backstabMultiplier = 2f; // Bonus damage when attacking from behind
}