using UnityEngine;

public class PlayerSpawner : MonoBehaviour
{
    [SerializeField] private GameObject knightPrefab;
    [SerializeField] private GameObject roguePrefab;
    [SerializeField] private GameObject magePrefab;
    [SerializeField] private GameObject archerPrefab;

    public GameObject SpawnPlayer(Vector3 spawnPosition)
    {
        string selectedClass = PlayerPrefs.GetString("selectedClass", "Knight");

        GameObject prefab = selectedClass switch
        {
            "Knight" => knightPrefab,
            "Rogue" => roguePrefab,
            "Mage" => magePrefab,
            "Archer" => archerPrefab,
            _ => knightPrefab
        };

        if (prefab == null)
        {
            Debug.LogError($"PlayerSpawner: No prefab assigned for class '{selectedClass}'");
            return null;
        }

        GameObject player = Instantiate(prefab, spawnPosition, Quaternion.identity);

        ApplyStartingStats(player);

        if (CameraController.Instance != null)
            CameraController.Instance.ChangeTarget(player.transform);

        return player;
    }

    private void ApplyStartingStats(GameObject player)
    {
        // Hook into whatever component holds your player's runtime stats
        // Example — replace PlayerStats with your actual component name
        // PlayerStats stats = player.GetComponent<PlayerStats>();
        // if (stats == null) return;
        // stats.health  = PlayerPrefs.GetFloat("startingHp",  100f);
        // stats.damage  = PlayerPrefs.GetFloat("startingAtk", 10f);
        // stats.speed   = PlayerPrefs.GetFloat("startingSpd", 5f);
    }
}