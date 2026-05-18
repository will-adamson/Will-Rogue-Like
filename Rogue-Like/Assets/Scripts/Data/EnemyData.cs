using UnityEngine;

[CreateAssetMenu(fileName = "EnemyData", menuName = "Scriptable Objects/EnemyData")]
public class EnemyData : ScriptableObject
{
    [Header("Identity")]
    public Sprite sprite;
    public new string name;
    public EnemyType enemyType;

    [Header("Base Stats")]
    public float health = 100f;
    public float damage = 10f;
    public float speed = 2f;
    public float defence = 0f;

    [Header("Combat")]
    public float attackRange = 1f;
    public float attackCooldown = 1f;
    public float detectionRange = 6f;

    [Header("Behaviour")]
    public bool isHoldPosition = false;

    [Header("Rewards")]
    public int xpReward = 10;

    [Header("Projectile")]
    public ProjectileData projectileData;
}