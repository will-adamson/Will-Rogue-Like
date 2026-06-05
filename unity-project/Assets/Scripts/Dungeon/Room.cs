using UnityEngine;

public class Room : MonoBehaviour
{
    private const string PLAYER_TAG = "Player";

    [SerializeField] private Transform[] chestSpawnPoints;

    private bool chestSpawned = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(PLAYER_TAG) && PlayerController.Instance != null)
            CameraController.Instance.ChangeTarget(PlayerController.Instance.transform);
    }

    public void TrySpawnChest(GameObject chestPrefab, float spawnChance)
    {
        if (chestSpawned) return;
        if (chestSpawnPoints == null || chestSpawnPoints.Length == 0) return;
        if (Random.value > spawnChance) return;

        Transform point = chestSpawnPoints[Random.Range(0, chestSpawnPoints.Length)];
        Instantiate(chestPrefab, point.position, Quaternion.identity, transform);
        chestSpawned = true;
    }
}