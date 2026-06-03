using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EnemyPrefabRegistry", menuName = "Scriptable Objects/Enemies/EnemyPrefabRegistry")]
public class EnemyPrefabRegistry : ScriptableObject
{
    [SerializeField] private List<EnemyTypePrefab> enemies = new List<EnemyTypePrefab>();

    [System.Serializable]
    private struct EnemyTypePrefab
    {
        public string type;
        public GameObject prefab;
    }

    public GameObject GetEnemyPrefab(string type)
    {
        foreach (EnemyTypePrefab enemy in enemies)
            if (enemy.type == type)
                return enemy.prefab;
        return null;
    }

    public bool HasEnemy(string type) => enemies.Exists(e => e.type == type);
}