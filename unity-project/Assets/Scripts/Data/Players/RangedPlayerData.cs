using UnityEngine;

[CreateAssetMenu(fileName = "RangedPlayerData", menuName = "Scriptable Objects/Players/Ranged Player Data")]
public class RangedPlayerData : PlayerData
{
    public ProjectileData arrowData;

    public float drawCooldown = 0.6f;
    public float arrowRange = 12f;
    public float arrowSpeed = 15f;

    /// <summary>TODO: implement multi-arrow shot.</summary>
    public int arrowsPerShot = 1;

    /// <summary>Spread angle in degrees between arrows when arrowsPerShot > 1.</summary>
    public float spreadAngle = 10f;

    /// <summary>TODO: implement charged shot mechanic.</summary>
    public float chargedShotMultiplier = 2f;
}
