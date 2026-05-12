using UnityEngine;

public class DoorTrigger : MonoBehaviour
{
    private const string PLAYER_TAG = "Player";
    private const float X_OFFSET_MIN = -4f;
    private const float X_OFFSET_MAX = 4f;
    private const float Y_OFFSET_MIN = -2f;
    private const float Y_OFFSET_MAX = 2f;

    private Vector3 endRoomPosition;
    private GameObject enemyPrefab;
    private int enemyCount;
    private Transform enemyParent;
    private bool hasTriggered;

    public void Initialize(Vector3 endRoomPosition, GameObject enemyPrefab, int enemyCount, Transform enemyParent)
    {
        this.endRoomPosition = endRoomPosition;
        this.enemyPrefab = enemyPrefab;
        this.enemyCount = enemyCount;
        this.enemyParent = enemyParent;

        hasTriggered = false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (hasTriggered) return;
        if (!other.CompareTag(PLAYER_TAG)) return;

        hasTriggered = true;

        for (int i = 0; i < enemyCount; i++)
        {
            Vector3 offset = new Vector3(Random.Range(X_OFFSET_MIN, X_OFFSET_MAX), Random.Range(Y_OFFSET_MIN, Y_OFFSET_MAX), 0);
            Instantiate(enemyPrefab, endRoomPosition + offset, Quaternion.identity, enemyParent);
        }

        gameObject.SetActive(false);
    }
}