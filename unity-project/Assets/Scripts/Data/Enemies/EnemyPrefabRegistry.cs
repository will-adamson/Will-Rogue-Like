using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Centralised registry for mapping enemy type identifiers to their prefabs.
/// Enables data-driven enemy spawning without hardcoded references.
/// </summary>
[CreateAssetMenu(fileName = "EnemyPrefabRegistry", menuName = "Scriptable Objects/Enemies/EnemyPrefabRegistry")]
public class EnemyPrefabRegistry : ScriptableObject
{
    #region Registry

    /// <summary>
    /// List of enemy type-to-prefab mappings.
    /// Edited in inspector to define available enemy types.
    /// </summary>
    [SerializeField]
    private List<EnemyTypePrefab> enemies = new List<EnemyTypePrefab>();

    #endregion

    #region Data Structure

    /// <summary>
    /// Associates an enemy type identifier with its instantiable prefab.
    /// </summary>
    [System.Serializable]
    private struct EnemyTypePrefab
    {
        /// <summary>
        /// Unique identifier for this enemy type.
        /// Used by EnemyWaveData to reference this enemy during spawning.
        /// Examples: "Slime", "LargeRat", "SmallRat".
        /// </summary>
        public string type;

        /// <summary>
        /// Prefab GameObject to instantiate when this enemy type is spawned.
        /// </summary>
        public GameObject prefab;
    }

    #endregion

    #region Public API

    /// <summary>
    /// Retrieves the prefab for a given enemy type.
    /// </summary>
    /// <param name="type">Enemy type identifier to look up.</param>
    /// <returns>Prefab GameObject if found; null if type not in registry.</returns>
    public GameObject GetEnemyPrefab(string type)
    {
        foreach (EnemyTypePrefab enemy in enemies)
            if (enemy.type == type)
                return enemy.prefab;
        return null;
    }

    /// <summary>
    /// Checks if a given enemy type exists in the registry.
    /// </summary>
    /// <param name="type">Enemy type identifier to check.</param>
    /// <returns>True if type is registered; false otherwise.</returns>
    public bool HasEnemy(string type) => enemies.Exists(e => e.type == type);

    #endregion
}
