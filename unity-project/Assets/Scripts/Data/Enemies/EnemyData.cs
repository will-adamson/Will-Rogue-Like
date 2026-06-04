using UnityEngine;

public abstract class EnemyData : ScriptableObject
{
    public Sprite sprite;
    public new string name;

    public float health = 100f;
    public float speed = 2f;
    public float defence = 0f;
    public float detectionRange = 6f;
    public float attackRange = 1f;

    /// <summary>When true, enemy stays stationary and does not patrol or chase.</summary>
    public bool isHoldPosition = false;

    public string attackName = "Attack";
    public float telegraphDuration = 2f;
    public TelegraphDanger danger = TelegraphDanger.Low;

    public int xpReward = 10;
}
