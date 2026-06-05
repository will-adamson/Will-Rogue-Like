using UnityEngine;

public class ChestSpawner : MonoBehaviour
{
    [SerializeField] private GameObject chestPrefab;
    [SerializeField] private float chestSpawnChance = 0.4f;

    public void OnRoomGenerated(Room room)
    {
        room.TrySpawnChest(chestPrefab, chestSpawnChance);
    }
}