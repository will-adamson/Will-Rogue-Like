using UnityEngine;

/// <summary>
/// Singleton that maintains a 2-D boolean walkability grid for the dungeon.
/// World positions are mapped to grid cells and vice-versa using a uniform cell size.
/// </summary>
/// <remarks>
/// Call <see cref="BakeWalls"/> once after all room prefabs have been placed to mark
/// every cell that overlaps a wall collider as non-walkable. The grid is centered on
/// the world origin: cell (width/2, height/2) corresponds to (0, 0) in world space.
/// </remarks>
public class GridManager : MonoBehaviour
{
    #region Singleton

    /// <summary>Shared instance; set during <see cref="Awake"/>.</summary>
    public static GridManager Instance;

    #endregion

    #region Inspector Fields

    /// <summary>Number of cells along the horizontal axis.</summary>
    [SerializeField] private int width = 2000;

    /// <summary>Number of cells along the vertical axis.</summary>
    [SerializeField] private int height = 2000;

    /// <summary>Side length of each square cell in world units.</summary>
    [SerializeField] private float cellSize = 1f;

    /// <summary>Layer mask used by <see cref="BakeWalls"/> to detect wall colliders.</summary>
    [SerializeField] private LayerMask wallLayerMask;

    #endregion

    #region Private State

    /// <summary>
    /// Flat 2-D array indexed [x, y]. <see langword="true"/> means the cell is passable.
    /// Initialised to all-true; wall cells are set to <see langword="false"/> by <see cref="BakeWalls"/>.
    /// </summary>
    private bool[,] walkable;

    #endregion

    #region Unity Lifecycle

    /// <summary>
    /// Enforces singleton pattern and pre-marks every cell as walkable.
    /// Duplicate instances are destroyed immediately.
    /// </summary>
    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;

        walkable = new bool[width, height];
        for (int x = 0; x < width; x++)
            for (int y = 0; y < height; y++)
                walkable[x, y] = true;
    }

    #endregion

    #region Public API

    /// <summary>
    /// Iterates every cell and performs a small <see cref="Physics2D.OverlapCircle"/> check
    /// at its world-space center. Cells that overlap a wall collider are marked non-walkable.
    /// </summary>
    /// <remarks>
    /// This is an expensive O(width × height) operation and should be called only once,
    /// after all room geometry has been instantiated.
    /// </remarks>
    public void BakeWalls()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Vector2 worldPos = GridToWorld(new Vector2Int(x, y));
                if (Physics2D.OverlapCircle(worldPos, cellSize * 0.4f, wallLayerMask))
                    walkable[x, y] = false;
            }
        }
    }

    /// <summary>
    /// Returns whether the cell at (<paramref name="x"/>, <paramref name="y"/>) is walkable.
    /// Out-of-bounds coordinates are treated as non-walkable.
    /// </summary>
    /// <param name="x">Grid column index.</param>
    /// <param name="y">Grid row index.</param>
    /// <returns>
    /// <see langword="true"/> if the cell is within bounds and not blocked by a wall;
    /// otherwise <see langword="false"/>.
    /// </returns>
    public bool IsWalkable(int x, int y)
    {
        if (x < 0 || x >= width || y < 0 || y >= height) return false;
        return walkable[x, y];
    }

    /// <summary>
    /// Converts a world-space position to the nearest grid cell index,
    /// accounting for the grid's centered origin.
    /// </summary>
    /// <param name="worldPos">Position in world space.</param>
    /// <returns>The grid cell that contains <paramref name="worldPos"/>.</returns>
    public Vector2Int WorldToGrid(Vector2 worldPos)
    {
        return new Vector2Int(
            Mathf.RoundToInt(worldPos.x / cellSize) + width / 2,
            Mathf.RoundToInt(worldPos.y / cellSize) + height / 2
        );
    }

    /// <summary>
    /// Converts a grid cell index to its world-space center position.
    /// </summary>
    /// <param name="gridPos">Cell index to convert.</param>
    /// <returns>The world-space center of the given cell.</returns>
    public Vector2 GridToWorld(Vector2Int gridPos)
    {
        return new Vector2(
            (gridPos.x - width / 2) * cellSize,
            (gridPos.y - height / 2) * cellSize
        );
    }

    #endregion
}