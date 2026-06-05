using UnityEngine;

[CreateAssetMenu(fileName = "ProjectileData", menuName = "Scriptable Objects/Projectile Data")]
public class ProjectileData : ScriptableObject
{
    public new string name;
    public GameObject projectilePrefab;

    public float damage = 10f;
    public float speed = 10f;
    public float range = 5f;
    public float cooldown = 0.3f;

    /// <summary>Number of instances pre-allocated for object pooling.</summary>
    public int poolSize = 10;
}
