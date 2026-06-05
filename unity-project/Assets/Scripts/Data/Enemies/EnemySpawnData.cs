using UnityEngine;

[System.Serializable]
public class EnemySpawn
{
    /// <summary>Must match a type registered in EnemyPrefabRegistry. Examples: "Large Rat", "Slime".</summary>
    public string enemyType;
    public int count;

    /// <summary>Enemies are randomly distributed within this area around the spawn center.</summary>
    public Vector2 spawnAreaSize;
}

[CreateAssetMenu(fileName = "EnemyWaveData", menuName = "Scriptable Objects/Waves/EnemyWaveData")]
public class EnemyWaveData : ScriptableObject
{
    [SerializeField] public EnemySpawn[] enemies;

    /// <summary>Seconds between each individual enemy spawn to stagger arrivals.</summary>
    [SerializeField] public float spawnDelay = 0.5f;
}
