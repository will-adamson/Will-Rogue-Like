using UnityEngine;

[System.Serializable]
public class EnemySpawn
{
    public string enemyType;
    public int count;
    public Vector2 spawnAreaSize;
}

[CreateAssetMenu(fileName = "EnemyWaveData", menuName = "Scriptable Objects/Waves/EnemyWaveData")]
public class EnemyWaveData : ScriptableObject
{
    [SerializeField] public EnemySpawn[] enemies;
    [SerializeField] public float spawnDelay = 0.5f;
}