using UnityEngine;

[CreateAssetMenu(fileName = "RangedEnemyData", menuName = "Scriptable Objects/Enemies/RangedEnemyData")]
public class RangedEnemyData : EnemyData
{
    [Header("Ranged")]
    public ProjectileData projectileData;
}