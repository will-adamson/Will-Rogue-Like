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

        if (prefab == null) return null;

        GameObject player = Instantiate(prefab, spawnPosition, Quaternion.identity);

        if (CameraController.Instance != null)
            CameraController.Instance.ChangeTarget(player.transform);

        return player;
    }
}