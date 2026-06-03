using UnityEngine;

public abstract class EnemyData : ScriptableObject
{
    [Header("Identity")]
    public Sprite sprite;
    public new string name;

    [Header("Base Stats")]
    public float health = 100f;
    public float speed = 2f;
    public float defence = 0f;

    [Header("Detection")]
    public float detectionRange = 6f;
    public float attackRange = 1f;

    [Header("Behaviour")]
    public bool isHoldPosition = false;

    [Header("Rewards")]
    public int xpReward = 10;
}