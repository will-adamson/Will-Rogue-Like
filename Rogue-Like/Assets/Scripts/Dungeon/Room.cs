using UnityEngine;

public class Room : MonoBehaviour
{
    private const string PLAYER_TAG = "Player";

    [Tooltip("Uncheck on roomUpDown and roomUpLeftRightDown prefabs")]
    public bool allowPlayerSpawn = true;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(PLAYER_TAG) && PlayerController.Instance != null)
            CameraController.Instance.ChangeTarget(PlayerController.Instance.transform);
    }
}