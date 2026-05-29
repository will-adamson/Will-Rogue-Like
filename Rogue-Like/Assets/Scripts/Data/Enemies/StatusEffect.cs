using UnityEngine;

[CreateAssetMenu(fileName = "StatusEffect", menuName = "Scriptable Objects/StatusEffect")]
public class StatusEffect : ScriptableObject
{
    public string effectName;
    public float duration = 2f;
    public float speedMultiplier = 1f;
    public float damagePerSecond = 0f;
}