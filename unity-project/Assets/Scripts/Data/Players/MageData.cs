using UnityEngine;

[CreateAssetMenu(fileName = "MagePlayerData", menuName = "Scriptable Objects/Players/MagePlayerData")]
public class MagePlayerData : PlayerData
{
    public ProjectileData projectileData;

    public float castCooldown = 0.3f;
    public float spellRange = 8f;
    public float spellSpeed = 10f;
    public int projectilesPerCast = 1;

    /// <summary>Spread angle in degrees between projectiles when projectilesPerCast > 1.</summary>
    public float spreadAngle = 15f;
}
