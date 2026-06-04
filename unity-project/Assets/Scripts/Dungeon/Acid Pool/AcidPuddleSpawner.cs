using UnityEngine;

/// <summary>
/// Spawns a set of <see cref="AcidPuddle"/> instances at predefined world positions when the scene starts.
/// </summary>
/// <remarks>
/// Populate <see cref="spawnPositions"/> in the Inspector to define where puddles appear.
/// All puddles are instantiated as children of this GameObject for a clean hierarchy.
/// To randomise puddle placement at runtime, consider replacing the fixed array with a
/// procedural selection from walkable grid cells provided by <see cref="GridManager"/>.
/// </remarks>
public class AcidPuddleSpawner : MonoBehaviour
{
    #region Inspector Fields

    [Header("Prefabs")]
    /// <summary>The <see cref="AcidPuddle"/> prefab to instantiate at each spawn point.</summary>
    [SerializeField] private GameObject acidPuddlePrefab;

    [Header("Spawn Points")]
    /// <summary>
    /// World-space positions where acid puddles will be created on <see cref="Start"/>.
    /// Each entry produces exactly one puddle instance.
    /// </summary>
    [SerializeField] private Vector2[] spawnPositions;

    #endregion

    #region Unity Lifecycle

    /// <summary>
    /// Instantiates <see cref="acidPuddlePrefab"/> at every position in <see cref="spawnPositions"/>,
    /// parenting each instance to this Transform.
    /// </summary>
    private void Start()
    {
        foreach (Vector2 position in spawnPositions)
        {
            Instantiate(acidPuddlePrefab, position, Quaternion.identity, transform);
        }
    }

    #endregion
}