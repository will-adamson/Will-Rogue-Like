using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Serializable container holding prefab references for every possible room connection shape.
/// Each field corresponds to a unique combination of open doorways (Up, Down, Left, Right).
/// </summary>
[System.Serializable]
public class RoomPrefabs
{
    /// <summary>Prefab for a room with only a downward exit.</summary>
    public GameObject roomDown;

    /// <summary>Prefab for a room with only a left exit.</summary>
    public GameObject roomLeft;

    /// <summary>Prefab for a room with left and down exits.</summary>
    public GameObject roomLeftDown;

    /// <summary>Prefab for a room with left and right exits.</summary>
    public GameObject roomLeftRight;

    /// <summary>Prefab for a room with left, right, and down exits.</summary>
    public GameObject roomLeftRightDown;

    /// <summary>Prefab for a room with only a right exit.</summary>
    public GameObject roomRight;

    /// <summary>Prefab for a room with right and down exits.</summary>
    public GameObject roomRightDown;

    /// <summary>Prefab for a room with only an upward exit.</summary>
    public GameObject roomUp;

    /// <summary>Prefab for a room with up and down exits.</summary>
    public GameObject roomUpDown;

    /// <summary>Prefab for a room with up and left exits.</summary>
    public GameObject roomUpLeft;

    /// <summary>Prefab for a room with up, left, and down exits.</summary>
    public GameObject roomUpLeftDown;

    /// <summary>Prefab for a room with up, left, and right exits.</summary>
    public GameObject roomUpLeftRight;

    /// <summary>Prefab for a room open in all four directions.</summary>
    public GameObject roomUpLeftRightDown;

    /// <summary>Prefab for a room with up and right exits.</summary>
    public GameObject roomUpRight;

    /// <summary>Prefab for a room with up, right, and down exits.</summary>
    public GameObject roomUpRightDown;
}

/// <summary>
/// Procedurally generates a dungeon layout at runtime by placing room markers, resolving
/// connection shapes, and instantiating the appropriate room prefabs.
/// </summary>
/// <remarks>
/// Generation runs in three phases:
/// <list type="number">
///   <item><description><see cref="GenerateLayout"/> - walks a random path to place marker GameObjects.</description></item>
///   <item><description><see cref="PlaceRoomPrefabs"/> - samples neighbors via <see cref="Physics2D.OverlapCircle"/> and instantiates matching prefabs.</description></item>
///   <item><description>Post-placement - bakes the nav grid, spawns enemies, and spawns the player.</description></item>
/// </list>
/// Requires <see cref="PlayerSpawner"/> and <see cref="EnemySpawner"/> components on the same GameObject.
/// </remarks>
[RequireComponent(typeof(PlayerSpawner), typeof(EnemySpawner))]
public class LevelGenerator : MonoBehaviour
{
    #region Inspector References

    [Header("References")]
    /// <summary>Parent GameObject that all dungeon content is nested under in the hierarchy.</summary>
    [SerializeField] private GameObject dungeonParent;

    /// <summary>Set of room prefabs covering every possible doorway combination.</summary>
    [SerializeField] private RoomPrefabs roomPrefabs;

    /// <summary>Transform whose position is moved step-by-step during layout generation.</summary>
    [SerializeField] private Transform generationPoint;

    /// <summary>Layer mask used to detect existing room markers via overlap checks.</summary>
    [SerializeField] private LayerMask roomLayerMask;

    [Header("Enemy Spawning")]
    /// <summary>
    /// Wave data assigned to each intermediate room in order.
    /// Rooms without a corresponding entry are left empty.
    /// </summary>
    [SerializeField] private EnemyWaveData[] roomWaves;

    [Header("Generation Settings")]
    /// <summary>Number of steps taken from the start room to the end room.</summary>
    [SerializeField] private int distanceToEnd;

    /// <summary>Horizontal distance in world units between adjacent room centers.</summary>
    [SerializeField] private float xOffset = 40f;

    /// <summary>Vertical distance in world units between adjacent room centers.</summary>
    [SerializeField] private float yOffset = 40f;

    #endregion

    #region Private State

    private PlayerSpawner playerSpawner;
    private EnemySpawner enemySpawner;

    private Vector3 startRoomPosition;
    private Vector3 endRoomPosition;
    private Vector3 playerSpawnPosition;
    private List<Vector3> roomPositions;
    private List<GameObject> markers;
    private Transform roomParent;
    private Direction direction;

    #endregion

    #region Unity Lifecycle

    /// <summary>
    /// Bootstraps the dungeon: generates layout, places prefabs, bakes walls,
    /// spawns enemies, and spawns the player.
    /// </summary>
    private void Start()
    {
        playerSpawner = GetComponent<PlayerSpawner>();
        enemySpawner = GetComponent<EnemySpawner>();

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
        playerSpawner.SpawnPlayer(playerSpawnPosition);
    }

    #endregion

    #region Generation

    /// <summary>
    /// Walks a random path of length <see cref="distanceToEnd"/> from the start position,
    /// placing a room marker at each step and recording positions for later prefab placement.
    /// Retries the current step (up to 100 times) if a chosen direction would overlap an
    /// existing marker.
    /// </summary>
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

    /// <summary>
    /// Instantiates the correct room prefab at every recorded position (start, intermediate, end)
    /// based on which neighboring grid cells contain markers.
    /// Also determines a safe player spawn position.
    /// </summary>
    private void PlaceRoomPrefabs()
    {
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

    /// <summary>
    /// Searches the intermediate rooms for one whose shape is safe for player spawning.
    /// Falls back to <paramref name="fallback"/> if no suitable room is found.
    /// </summary>
    /// <param name="fallback">Position returned when no intermediate room qualifies.</param>
    /// <returns>A world-space position suitable for spawning the player.</returns>
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

    /// <summary>
    /// Returns <see langword="true"/> when the given doorway combination is safe for player spawning.
    /// Corridors that are purely vertical (up+down only) or fully cross-shaped are considered unsafe
    /// because the player could immediately enter combat on two axes.
    /// </summary>
    /// <param name="up">Whether the room has an upward neighbor.</param>
    /// <param name="down">Whether the room has a downward neighbor.</param>
    /// <param name="left">Whether the room has a left neighbor.</param>
    /// <param name="right">Whether the room has a right neighbor.</param>
    /// <returns><see langword="true"/> if the shape is safe; otherwise <see langword="false"/>.</returns>
    private bool IsSpawnSafeShape(bool up, bool down, bool left, bool right)
    {
        bool isUpDown = up && down && !left && !right;
        bool isUpLeftRightDown = up && down && left && right;
        return !isUpDown && !isUpLeftRightDown;
    }

    /// <summary>
    /// Spawns enemy waves into each intermediate room that has a corresponding
    /// entry in <see cref="roomWaves"/>.
    /// </summary>
    private void SpawnEnemies()
    {
        for (int i = 0; i < roomPositions.Count; i++)
        {
            if (i < roomWaves.Length && roomWaves[i] != null)
                enemySpawner.SpawnWave(roomWaves[i], roomPositions[i]);
        }
    }

    /// <summary>
    /// Destroys all temporary marker GameObjects created during <see cref="GenerateLayout"/>.
    /// </summary>
    private void CleanupMarkers()
    {
        foreach (GameObject marker in markers) Destroy(marker);
        markers.Clear();
    }

    #endregion

    #region Room Prefab Placement

    /// <summary>
    /// Instantiates the room prefab whose doorway shape matches the neighbors present
    /// at <paramref name="roomPosition"/>.
    /// </summary>
    /// <param name="roomPosition">World-space center of the room to create.</param>
    /// <returns>
    /// The instantiated room GameObject, or <see langword="null"/> if no matching prefab exists.
    /// </returns>
    public GameObject CreateRoomOutline(Vector3 roomPosition)
    {
        bool up = Physics2D.OverlapCircle(roomPosition + new Vector3(0, yOffset, 0), 0.2f, roomLayerMask);
        bool down = Physics2D.OverlapCircle(roomPosition + new Vector3(0, -yOffset, 0), 0.2f, roomLayerMask);
        bool left = Physics2D.OverlapCircle(roomPosition + new Vector3(-xOffset, 0, 0), 0.2f, roomLayerMask);
        bool right = Physics2D.OverlapCircle(roomPosition + new Vector3(xOffset, 0, 0), 0.2f, roomLayerMask);

        GameObject prefab = GetRoomPrefab(up, down, left, right);
        if (prefab == null) return null;

        return Instantiate(prefab, roomPosition, Quaternion.identity, roomParent);
    }

    /// <summary>
    /// Maps a doorway bitmask (up/down/left/right) to the correct room prefab from
    /// <see cref="roomPrefabs"/>.
    /// </summary>
    /// <param name="up">Whether the room opens upward.</param>
    /// <param name="down">Whether the room opens downward.</param>
    /// <param name="left">Whether the room opens to the left.</param>
    /// <param name="right">Whether the room opens to the right.</param>
    /// <returns>The matching prefab, or <see langword="null"/> if the combination is unrecognised.</returns>
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

    #endregion

    #region Helpers

    /// <summary>
    /// Creates an invisible trigger-only marker GameObject at <paramref name="position"/>
    /// on the room layer so overlap checks can detect it during layout generation.
    /// </summary>
    /// <param name="position">World-space position for the marker.</param>
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

    /// <summary>
    /// Translates <see cref="generationPoint"/> by one grid step in the current
    /// <see cref="direction"/>.
    /// </summary>
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

    /// <summary>Returns a random cardinal <see cref="Direction"/>.</summary>
    /// <returns>One of Up, Down, Left, or Right chosen uniformly at random.</returns>
    private static Direction RandomDirection() => (Direction)Random.Range(0, 4);

    /// <summary>
    /// Extracts the index of the first set bit from a <see cref="LayerMask"/>,
    /// converting it to a Unity layer integer.
    /// </summary>
    /// <param name="mask">The layer mask to read.</param>
    /// <returns>The layer index corresponding to the lowest set bit in <paramref name="mask"/>.</returns>
    private static int GetLayerFromMask(LayerMask mask)
    {
        int value = mask.value, layer = 0;
        while (value > 1) { value >>= 1; layer++; }
        return layer;
    }

    #endregion
}