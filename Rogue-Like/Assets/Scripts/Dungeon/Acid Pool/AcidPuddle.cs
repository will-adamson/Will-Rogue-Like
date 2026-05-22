using System.Collections;
using UnityEngine;

public class AcidPuddle : MonoBehaviour
{
    [Header("Damage")]
    [SerializeField] private float damagePerTick = 5f;
    [SerializeField] private float tickInterval = 1f;

    private bool playerInside = false;
    private Coroutine damageCoroutine;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        playerInside = true;
        damageCoroutine = StartCoroutine(DamageRoutine(other.gameObject));
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        playerInside = false;
        if (damageCoroutine != null)
            StopCoroutine(damageCoroutine);
    }

    private IEnumerator DamageRoutine(GameObject player)
    {
        PlayerController playerController = player.GetComponent<PlayerController>();
        if (playerController == null) yield break;

        while (playerInside)
        {
            playerController.TakeDamage(damagePerTick);
            yield return new WaitForSeconds(tickInterval);
        }
    }
}