using UnityEngine;

public abstract class PlayerData : ScriptableObject
{
    public Sprite sprite;
    public new string name;
    public float health = 100f;
    public float stamina = 100f;
    public float damage = 10f;
    public float speed = 5f;
    public float defence = 0f;

    /// <summary>Range: 0–1 (not 0–100).</summary>
    [Range(0f, 1f)]
    public float critChance = 0f;

    public float critMultiplier = 1.5f;
    public float experienceToNextLevel = 100f;
}
