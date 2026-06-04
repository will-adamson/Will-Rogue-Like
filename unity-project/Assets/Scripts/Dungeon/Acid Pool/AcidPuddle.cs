using System.Collections;
using UnityEngine;

public class AcidPuddle : MonoBehaviour
{
    [Header("Damage")]
    [SerializeField] private float damagePerTick = 5f;
    [SerializeField] private float tickInterval = 1f;

    [Header("Fade")]
    /// <summary>When true, the puddle never fades and must be destroyed externally.</summary>
    [SerializeField] private bool isPermanent = false;
    [SerializeField] private float minLifetime = 3f;
    [SerializeField] private float maxLifetime = 10f;
    [SerializeField] private float fadeDuration = 1.5f;

    // Tracked via flag rather than re-querying the collider each tick.
    private bool playerInside = false;
    private Coroutine damageCoroutine;
    private SpriteRenderer sr;

    private void Start()
    {
        sr = GetComponentInChildren<SpriteRenderer>();

        if (!isPermanent)
            StartCoroutine(LifetimeRoutine());
    }

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

    private IEnumerator LifetimeRoutine()
    {
        float randomLifetime = Random.Range(minLifetime, maxLifetime);
        yield return new WaitForSeconds(randomLifetime);

        float elapsed = 0f;
        Color startColor = sr.color;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, elapsed / fadeDuration);
            sr.color = new Color(startColor.r, startColor.g, startColor.b, alpha);
            yield return null;
        }

        Destroy(gameObject);
    }
}