using UnityEngine;

public class PlayerSpawner : MonoBehaviour
{
    [SerializeField] private GameObject dwarvenFighterPrefab;
    [SerializeField] private GameObject humanWizardPrefab;
    [SerializeField] private GameObject elfRangerPrefab;

    public GameObject SpawnPlayer(Vector3 spawnPosition)
    {
        string selectedClass = PlayerPrefs.GetString("selectedClass", "Dwarven Fighter");

        GameObject prefab = selectedClass switch
        {
            "Dwarven Fighter" => dwarvenFighterPrefab,
            "Human Wizard" => humanWizardPrefab,
            "Elf Ranger" => elfRangerPrefab,
            _ => dwarvenFighterPrefab
        };

        if (prefab == null) return null;

        GameObject player = Instantiate(prefab, spawnPosition, Quaternion.identity);

        if (CameraController.Instance != null)
            CameraController.Instance.ChangeTarget(player.transform);

        return player;
    }
}