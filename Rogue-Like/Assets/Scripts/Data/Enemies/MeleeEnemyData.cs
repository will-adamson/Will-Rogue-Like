using UnityEngine;

[CreateAssetMenu(fileName = "MeleeEnemyData", menuName = "Scriptable Objects/Enemies/MeleeEnemyData")]
public class MeleeEnemyData : EnemyData
{
    [Header("Melee")]
    public float knockbackForce = 2f;
    public float attackCooldown = 1f;
}