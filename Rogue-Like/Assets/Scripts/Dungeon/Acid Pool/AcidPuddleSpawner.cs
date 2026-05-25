using UnityEngine;

public class AcidPuddleSpawner : MonoBehaviour
{
    [Header("Prefabs")]
    [SerializeField] private GameObject acidPuddlePrefab;

    [Header("Spawn Points")]
    [SerializeField] private Vector2[] spawnPositions;

    private void Start()
    {
        foreach (Vector2 position in spawnPositions)
        {
            Instantiate(acidPuddlePrefab, position, Quaternion.identity, transform);
        }
    }
}