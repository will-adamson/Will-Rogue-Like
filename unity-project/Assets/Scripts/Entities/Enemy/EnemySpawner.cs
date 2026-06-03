using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private EnemyPrefabRegistry enemyRegistry;
    [SerializeField] private Transform enemyParent;

    private Tilemap currentTilemap;

    public void SpawnWave(EnemyWaveData waveData, Vector3 roomCenter)
    {
        currentTilemap = FindNearestTilemap(roomCenter);
        if (currentTilemap == null)
        {
            return;
        }

        StartCoroutine(SpawnWaveRoutine(waveData, roomCenter));
    }

    private Tilemap FindNearestTilemap(Vector3 position)
    {
        Tilemap[] tilemaps = FindObjectsByType<Tilemap>(FindObjectsInactive.Exclude);
        Tilemap nearest = null;
        float nearestDist = float.MaxValue;

        foreach (Tilemap tm in tilemaps)
        {
            float dist = Vector3.Distance(tm.transform.position, position);
            if (dist < nearestDist)
            {
                nearestDist = dist;
                nearest = tm;
            }
        }

        return nearest;
    }

    private System.Collections.IEnumerator SpawnWaveRoutine(EnemyWaveData waveData, Vector3 roomCenter)
    {
        foreach (EnemySpawn spawn in waveData.enemies)
        {
            for (int i = 0; i < spawn.count; i++)
            {
                SpawnEnemy(spawn.enemyType, roomCenter, spawn.spawnAreaSize);
                yield return new WaitForSeconds(waveData.spawnDelay);
            }
        }
    }

    private void SpawnEnemy(string enemyType, Vector3 center, Vector2 areaSize)
    {
        GameObject prefab = enemyRegistry.GetEnemyPrefab(enemyType);
        if (prefab == null) return;

        Vector3 spawnPos = FindValidSpawnPosition(center, areaSize);
        if (spawnPos == Vector3.zero) return;

        Instantiate(prefab, spawnPos, Quaternion.identity, enemyParent);
    }

    private Vector3 FindValidSpawnPosition(Vector3 center, Vector2 areaSize)
    {
        if (currentTilemap == null) return center;

        int attempts = 20;

        for (int i = 0; i < attempts; i++)
        {
            Vector3 randomOffset = new Vector3(
                Random.Range(-areaSize.x * 0.5f, areaSize.x * 0.5f),
                Random.Range(-areaSize.y * 0.5f, areaSize.y * 0.5f),
                0
            );

            Vector3 spawnPos = center + randomOffset;
            Vector3Int cellPos = currentTilemap.WorldToCell(spawnPos);
            TileBase tile = currentTilemap.GetTile(cellPos);

            if (tile != null)
            {
                Tile tilemapTile = tile as Tile;
                if (tilemapTile != null && tilemapTile.colliderType == Tile.ColliderType.None)
                    return spawnPos;
            }
        }

        return center;
    }
}