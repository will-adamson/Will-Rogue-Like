using UnityEngine;

/// <summary>
/// Represents a dungeon room trigger volume. When the player enters the collider,
/// the camera target is switched to the player so the camera re-centers on them.
/// </summary>
/// <remarks>
/// Attach to a GameObject with a 2-D trigger collider that covers the interior of
/// the room. The component relies on <see cref="CameraController.Instance"/> and
/// <see cref="PlayerController.Instance"/> being available in the scene.
/// </remarks>
public class Room : MonoBehaviour
{
    #region Constants

    /// <summary>Tag that identifies the player GameObject for trigger comparisons.</summary>
    private const string PLAYER_TAG = "Player";

    #endregion

    #region Trigger Handling

    /// <summary>
    /// Called when a 2-D collider enters this room's trigger zone.
    /// If the collider belongs to the player, updates the camera to follow the player.
    /// </summary>
    /// <param name="collision">The collider that entered the trigger.</param>
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(PLAYER_TAG) && PlayerController.Instance != null)
            CameraController.Instance.ChangeTarget(PlayerController.Instance.transform);
    }

    #endregion
}