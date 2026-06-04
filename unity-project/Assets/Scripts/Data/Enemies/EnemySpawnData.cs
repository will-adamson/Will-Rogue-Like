using UnityEngine;

/// <summary>
/// Defines a single enemy spawn entry within a wave.
/// Specifies enemy type, spawn count, and area size for distribution.
/// </summary>
[System.Serializable]
public class EnemySpawn
{
    #region Enemy Configuration

    /// <summary>
    /// Type identifier matching an entry in EnemyPrefabRegistry.
    /// Examples: "Slime", "LargeRat", "SmallRat".
    /// </summary>
    public string enemyType;

    /// <summary>
    /// Number of enemies of this type to spawn in the wave.
    /// </summary>
    public int count;

    /// <summary>
    /// Area dimensions in units where enemies can randomly spawn.
    /// Prevents all enemies from spawning at the exact center position.
    /// </summary>
    public Vector2 spawnAreaSize;

    #endregion
}

/// <summary>
/// Defines a complete enemy wave for a single room.
/// Contains multiple enemy spawn entries and timing configuration.
/// </summary>
[CreateAssetMenu(fileName = "EnemyWaveData", menuName = "Scriptable Objects/Waves/EnemyWaveData")]
public class EnemyWaveData : ScriptableObject
{
    #region Wave Composition

    /// <summary>
    /// Array of enemy spawn entries defining what enemies appear in this wave.
    /// Each entry specifies enemy type, count, and spawn area.
    /// </summary>
    [SerializeField]
    public EnemySpawn[] enemies;

    #endregion

    #region Timing

    /// <summary>
    /// Delay in seconds between spawning consecutive enemies.
    /// Creates staggered arrival for varied combat pacing.
    /// </summary>
    [SerializeField]
    public float spawnDelay = 0.5f;

    #endregion
}
