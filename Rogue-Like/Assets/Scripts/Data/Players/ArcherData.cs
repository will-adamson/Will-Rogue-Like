using UnityEngine;

[CreateAssetMenu(fileName = "ArcherData", menuName = "Scriptable Objects/Players/ArcherData")]
public class ArcherData : PlayerData
{
    [Header("Projectile")]
    public ProjectileData arrowData;

    [Header("Archer Stats")]
    public float drawCooldown = 0.6f;
    public float arrowRange = 12f;
    public float arrowSpeed = 15f;
    public int arrowsPerShot = 1; // TODO: For future multi-arrow shot mechanic
    public float spreadAngle = 10f;
    public float chargedShotMultiplier = 2f; // TODO: For future charged shot mechanic
}