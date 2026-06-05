using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EnemyPrefabRegistry", menuName = "Scriptable Objects/Enemies/Enemy Prefab Registry")]
public class EnemyPrefabRegistry : ScriptableObject
{
    [SerializeField]
    private List<EnemyTypePrefab> enemies = new List<EnemyTypePrefab>();

    [System.Serializable]
    private struct EnemyTypePrefab
    {
        /// <summary>Must match the type string used in EnemyWaveData. Examples: "Large Rat", "Slime".</summary>
        public string type;
        public GameObject prefab;
    }

    /// <returns>Prefab for the given type, or null if not registered.</returns>
    public GameObject GetEnemyPrefab(string type)
    {
        foreach (EnemyTypePrefab enemy in enemies)
            if (enemy.type == type)
                return enemy.prefab;
        return null;
    }

    public bool HasEnemy(string type) => enemies.Exists(e => e.type == type);
}
