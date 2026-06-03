using System.Collections;
using UnityEngine;

/// <summary>
/// Hazard that deals periodic damage to the player while they remain inside its trigger,
/// then optionally fades out and destroys itself after a random lifetime.
/// </summary>
/// <remarks>
/// The puddle tracks whether the player is inside using a boolean flag rather than
/// re-querying the collider each tick, which keeps the damage coroutine lightweight.
/// Set <see cref="isPermanent"/> to <see langword="true"/> for puddles that should
/// persist indefinitely (e.g. environmental hazards placed in the editor).
/// </remarks>
public class AcidPuddle : MonoBehaviour
{
    #region Inspector Fields

    [Header("Damage")]
    /// <summary>Health points deducted from the player on each damage tick.</summary>
    [SerializeField] private float damagePerTick = 5f;

    /// <summary>Seconds between successive damage applications while the player is inside.</summary>
    [SerializeField] private float tickInterval = 1f;

    [Header("Fade")]
    /// <summary>
    /// When <see langword="true"/>, the puddle never fades and must be destroyed externally.
    /// When <see langword="false"/>, <see cref="LifetimeRoutine"/> handles automatic cleanup.
    /// </summary>
    [SerializeField] private bool isPermanent = false;

    /// <summary>Minimum seconds the puddle remains fully opaque before fading. Range: 0 to <see cref="maxLifetime"/>.</summary>
    [SerializeField] private float minLifetime = 3f;

    /// <summary>Maximum seconds the puddle remains fully opaque before fading. Range: <see cref="minLifetime"/> to ∞.</summary>
    [SerializeField] private float maxLifetime = 10f;

    /// <summary>Duration in seconds of the alpha fade-out animation before the GameObject is destroyed.</summary>
    [SerializeField] private float fadeDuration = 1.5f;

    #endregion

    #region Private State

    /// <summary><see langword="true"/> while the player's collider overlaps this trigger.</summary>
    private bool playerInside = false;

    /// <summary>Reference to the running damage coroutine, kept so it can be stopped on exit.</summary>
    private Coroutine damageCoroutine;

    /// <summary>Cached <see cref="SpriteRenderer"/> used for the fade-out alpha animation.</summary>
    private SpriteRenderer sr;

    #endregion

    #region Unity Lifecycle

    /// <summary>
    /// Caches the child <see cref="SpriteRenderer"/> and starts <see cref="LifetimeRoutine"/>
    /// if the puddle is not permanent.
    /// </summary>
    private void Start()
    {
        sr = GetComponentInChildren<SpriteRenderer>();

        if (!isPermanent)
            StartCoroutine(LifetimeRoutine());
    }

    #endregion

    #region Trigger Handling

    /// <summary>
    /// Called when a 2-D collider enters this puddle's trigger.
    /// Begins <see cref="DamageRoutine"/> if the collider belongs to the player.
    /// </summary>
    /// <param name="other">The collider that entered the trigger.</param>
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        playerInside = true;
        damageCoroutine = StartCoroutine(DamageRoutine(other.gameObject));
    }

    /// <summary>
    /// Called when a 2-D collider exits this puddle's trigger.
    /// Stops <see cref="DamageRoutine"/> if the collider belongs to the player.
    /// </summary>
    /// <param name="other">The collider that exited the trigger.</param>
    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        playerInside = false;
        if (damageCoroutine != null)
            StopCoroutine(damageCoroutine);
    }

    #endregion

    #region Coroutines

    /// <summary>
    /// Repeatedly applies <see cref="damagePerTick"/> to the player every
    /// <see cref="tickInterval"/> seconds for as long as <see cref="playerInside"/> is true.
    /// </summary>
    /// <param name="player">The player GameObject whose <see cref="PlayerController"/> receives damage.</param>
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

    /// <summary>
    /// Waits for a random duration between <see cref="minLifetime"/> and <see cref="maxLifetime"/>,
    /// then linearly fades the sprite's alpha to zero over <see cref="fadeDuration"/> seconds
    /// before destroying the GameObject.
    /// </summary>
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

    #endregion
}