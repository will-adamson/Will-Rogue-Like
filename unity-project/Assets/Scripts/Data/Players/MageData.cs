
using UnityEngine;

[CreateAssetMenu(fileName = "MageData", menuName = "Scriptable Objects/Players/MageData")]
public class MageData : PlayerData
{
    [Header("Projectile")]
    public ProjectileData projectileData;

    [Header("Mage Stats")]
    public float castCooldown = 0.3f;
    public float spellRange = 8f;
    public float spellSpeed = 10f;
    public int projectilesPerCast = 1;
    public float spreadAngle = 15f;
}