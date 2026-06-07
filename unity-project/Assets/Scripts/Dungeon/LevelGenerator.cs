using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class RoomPrefabs
{
    public GameObject roomDown, roomLeft, roomLeftDown, roomLeftRight,
        roomLeftRightDown, roomRight, roomRightDown, roomUp, roomUpDown,
        roomUpLeft, roomUpLeftDown, roomUpLeftRight, roomUpLeftRightDown,
        roomUpRight, roomUpRightDown;
}

[RequireComponent(typeof(PlayerSpawner), typeof(EnemySpawner), typeof(ChestSpawner))]
public class LevelGenerator : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject dungeonParent;
    [SerializeField] private RoomPrefabs roomPrefabs;
    [SerializeField] private Transform generationPoint;
    [SerializeField] private LayerMask roomLayerMask;

    [Header("Enemy Spawning")]
    [SerializeField] private EnemyWaveData[] roomWaves;

    [Header("Generation Settings")]
    [SerializeField] private int distanceToEnd;
    [SerializeField] private float xOffset = 18f;
    [SerializeField] private float yOffset = 10f;

    private PlayerSpawner playerSpawner;
    private EnemySpawner enemySpawner;
    private ChestSpawner chestSpawner;

    private Vector3 startRoomPosition;
    private Vector3 endRoomPosition;
    private Vector3 playerSpawnPosition;
    private List<Vector3> roomPositions;
    private List<Room> spawnedRooms = new List<Room>();
    private List<GameObject> markers;
    private Transform roomParent;
    private Direction direction;

    private void Start()
    {
        playerSpawner = GetComponent<PlayerSpawner>();
        enemySpawner = GetComponent<EnemySpawner>();
        chestSpawner = GetComponent<ChestSpawner>();

        if (ObjectPoolManager.Instance == null)
            new GameObject("Object Pool Manager").AddComponent<ObjectPoolManager>();

        roomPositions = new List<Vector3>();
        markers = new List<GameObject>();
        roomParent = new GameObject("Rooms").transform;
        roomParent.SetParent(dungeonParent.transform);

        GenerateLayout();
        PlaceRoomPrefabs();
        CleanupMarkers();

        GridManager.Instance.BakeWalls();

        SpawnEnemies();
        SpawnChests();
        playerSpawner.SpawnPlayer(playerSpawnPosition);
    }

    private void GenerateLayout()
    {
        startRoomPosition = generationPoint.position;
        PlaceMarker(startRoomPosition);

        direction = RandomDirection();
        MoveGenerationPoint();

        for (int i = 0; i < distanceToEnd; i++)
        {
            Vector3 pos = generationPoint.position;
            PlaceMarker(pos);

            if (i == distanceToEnd - 1)
                endRoomPosition = pos;
            else
                roomPositions.Add(pos);

            Vector3 lastValid = generationPoint.position;
            direction = RandomDirection();
            MoveGenerationPoint();

            // Retry up to 100 times if the chosen direction overlaps an existing marker.
            int safetyLimit = 100;
            while (Physics2D.OverlapCircle(generationPoint.position, 0.2f, roomLayerMask))
            {
                generationPoint.position = lastValid;
                direction = RandomDirection();
                MoveGenerationPoint();
                if (--safetyLimit <= 0) break;
            }
        }
    }

    private void PlaceRoomPrefabs()
    {
        spawnedRooms = new List<Room>();

        CreateRoomOutline(startRoomPosition);

        bool up = Physics2D.OverlapCircle(startRoomPosition + new Vector3(0, yOffset, 0), 0.2f, roomLayerMask);
        bool down = Physics2D.OverlapCircle(startRoomPosition + new Vector3(0, -yOffset, 0), 0.2f, roomLayerMask);
        bool left = Physics2D.OverlapCircle(startRoomPosition + new Vector3(-xOffset, 0, 0), 0.2f, roomLayerMask);
        bool right = Physics2D.OverlapCircle(startRoomPosition + new Vector3(xOffset, 0, 0), 0.2f, roomLayerMask);

        playerSpawnPosition = IsSpawnSafeShape(up, down, left, right)
            ? startRoomPosition
            : FindValidSpawnPosition(startRoomPosition);

        foreach (Vector3 pos in roomPositions)
            CreateRoomOutline(pos);

        CreateRoomOutline(endRoomPosition);
    }

    private Vector3 FindValidSpawnPosition(Vector3 fallback)
    {
        foreach (Vector3 pos in roomPositions)
        {
            bool up = Physics2D.OverlapCircle(pos + new Vector3(0, yOffset, 0), 0.2f, roomLayerMask);
            bool down = Physics2D.OverlapCircle(pos + new Vector3(0, -yOffset, 0), 0.2f, roomLayerMask);
            bool left = Physics2D.OverlapCircle(pos + new Vector3(-xOffset, 0, 0), 0.2f, roomLayerMask);
            bool right = Physics2D.OverlapCircle(pos + new Vector3(xOffset, 0, 0), 0.2f, roomLayerMask);

            if (IsSpawnSafeShape(up, down, left, right))
                return pos;
        }

        return fallback;
    }

    // Purely vertical (up+down) and fully cross-shaped rooms are unsafe spawn points
    // because the player would immediately be exposed on multiple axes.
    private bool IsSpawnSafeShape(bool up, bool down, bool left, bool right)
    {
        bool isUpDown = up && down && !left && !right;
        bool isUpLeftRightDown = up && down && left && right;
        return !isUpDown && !isUpLeftRightDown;
    }

    private void SpawnEnemies()
    {
        for (int i = 0; i < roomPositions.Count; i++)
        {
            if (i < roomWaves.Length && roomWaves[i] != null)
                enemySpawner.SpawnWave(roomWaves[i], roomPositions[i]);
        }
    }

    private void SpawnChests()
    {
        foreach (Room room in spawnedRooms)
            chestSpawner.OnRoomGenerated(room);
    }

    private void CleanupMarkers()
    {
        foreach (GameObject marker in markers) Destroy(marker);
        markers.Clear();
    }

    public GameObject CreateRoomOutline(Vector3 roomPosition)
    {
        bool up = Physics2D.OverlapCircle(roomPosition + new Vector3(0, yOffset, 0), 0.2f, roomLayerMask);
        bool down = Physics2D.OverlapCircle(roomPosition + new Vector3(0, -yOffset, 0), 0.2f, roomLayerMask);
        bool left = Physics2D.OverlapCircle(roomPosition + new Vector3(-xOffset, 0, 0), 0.2f, roomLayerMask);
        bool right = Physics2D.OverlapCircle(roomPosition + new Vector3(xOffset, 0, 0), 0.2f, roomLayerMask);

        GameObject prefab = GetRoomPrefab(up, down, left, right);
        if (prefab == null) return null;

        GameObject instance = Instantiate(prefab, roomPosition, Quaternion.identity, roomParent);

        Room room = instance.GetComponent<Room>();
        if (room != null && roomPosition != startRoomPosition && roomPosition != endRoomPosition)
            spawnedRooms.Add(room);

        return instance;
    }

    private GameObject GetRoomPrefab(bool up, bool down, bool left, bool right)
    {
        if (up && !down && !left && !right) return roomPrefabs.roomUp;
        if (!up && down && !left && !right) return roomPrefabs.roomDown;
        if (!up && !down && left && !right) return roomPrefabs.roomLeft;
        if (!up && !down && !left && right) return roomPrefabs.roomRight;

        if (up && down && !left && !right) return roomPrefabs.roomUpDown;
        if (up && !down && left && !right) return roomPrefabs.roomUpLeft;
        if (up && !down && !left && right) return roomPrefabs.roomUpRight;
        if (!up && down && left && !right) return roomPrefabs.roomLeftDown;
        if (!up && down && !left && right) return roomPrefabs.roomRightDown;
        if (!up && !down && left && right) return roomPrefabs.roomLeftRight;

        if (up && down && left && !right) return roomPrefabs.roomUpLeftDown;
        if (up && down && !left && right) return roomPrefabs.roomUpRightDown;
        if (up && !down && left && right) return roomPrefabs.roomUpLeftRight;
        if (!up && down && left && right) return roomPrefabs.roomLeftRightDown;
        if (up && down && left && right) return roomPrefabs.roomUpLeftRightDown;

        return null;
    }

    private void PlaceMarker(Vector3 position)
    {
        GameObject marker = new GameObject("RoomMarker");
        marker.transform.position = position;
        marker.layer = GetLayerFromMask(roomLayerMask);

        CircleCollider2D col = marker.AddComponent<CircleCollider2D>();
        col.isTrigger = true;
        col.radius = 0.1f;

        markers.Add(marker);
    }

    private void MoveGenerationPoint()
    {
        generationPoint.position += direction switch
        {
            Direction.Up => new Vector3(0, yOffset, 0),
            Direction.Down => new Vector3(0, -yOffset, 0),
            Direction.Left => new Vector3(-xOffset, 0, 0),
            Direction.Right => new Vector3(xOffset, 0, 0),
            _ => Vector3.zero
        };
    }

    private static Direction RandomDirection() => (Direction)Random.Range(0, 4);

    // Extracts the layer index from the lowest set bit of the mask value.
    private static int GetLayerFromMask(LayerMask mask)
    {
        int value = mask.value, layer = 0;
        while (value > 1) { value >>= 1; layer++; }
        return layer;
    }
}