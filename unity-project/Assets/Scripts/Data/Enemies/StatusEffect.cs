using UnityEngine;

[CreateAssetMenu(fileName = "StatusEffect", menuName = "Scriptable Objects/StatusEffect")]
public class StatusEffect : ScriptableObject
{
    public string effectName;
    public float duration = 2f;

    /// <summary>Speed multiplier while active. Range: 0–1. Example: 0.5 = 50% slow.</summary>
    public float speedMultiplier = 1f;

    /// <summary>Damage per second for DoT effects (poison, burn, etc.). 0 = no DoT.</summary>
    public float damagePerSecond = 0f;

    public bool bypassDefence = false;
    public int maxStacks = 1;
}
