using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
/// HUD component that manages the enemy telegraph panel: enemy name, incoming
/// attack label, danger severity pip, countdown timer, and a draining progress bar.
/// </summary>
/// <remarks>
/// Implements <see cref="IHUDComponent"/>. Call <see cref="Init"/> once after the
/// UI Document layout is ready. The panel is hidden by default; show it via
/// <see cref="Show"/> when an enemy begins telegraphing an attack and hide it
/// via <see cref="Hide"/> when the attack resolves or is interrupted.
/// Drive the countdown each frame via <see cref="Tick"/>, or call
/// <see cref="SetProgress"/> directly if managing time externally.
/// </remarks>
public class TelegraphComponent : IHUDComponent
{
    #region Cached UI Elements

    /// <summary>Root container; shown and hidden via the <c>telegraph-visible</c> USS class.</summary>
    private VisualElement telegraphContainer;

    /// <summary>Label displaying the attacking enemy's name in uppercase.</summary>
    private Label telegraphEnemyName;

    /// <summary>Label displaying the name of the incoming attack.</summary>
    private Label telegraphAttackName;

    /// <summary>Label displaying the remaining time before the attack lands.</summary>
    private Label telegraphTimer;

    /// <summary>Small pip whose USS class is swapped to reflect the attack's danger level.</summary>
    private VisualElement telegraphDangerPip;

    /// <summary>Fill element whose width percentage drains from 100% to 0% as the attack window closes.</summary>
    private VisualElement telegraphBarFill;

    #endregion

    #region Private Fields

    /// <summary>Total duration of the current telegraph window in seconds.</summary>
    private float totalDuration;

    /// <summary>Remaining time in seconds; decremented by <see cref="Tick"/>.</summary>
    private float remainingTime;

    /// <summary>Danger level applied to the current telegraph; drives pip and bar fill colour.</summary>
    private TelegraphDanger currentDanger;

    #endregion

    #region IHUDComponent

    /// <summary>
    /// Resolves and caches all UI element references from <paramref name="root"/>.
    /// Must be called before any other method.
    /// </summary>
    /// <param name="root">Root <see cref="VisualElement"/> of the HUD UI Document.</param>
    public void Init(VisualElement root)
    {
        telegraphContainer = root.Q<VisualElement>("telegraph-container");
        telegraphEnemyName = root.Q<Label>("telegraph-enemy-name");
        telegraphAttackName = root.Q<Label>("telegraph-attack-name");
        telegraphTimer = root.Q<Label>("telegraph-timer");
        telegraphDangerPip = root.Q<VisualElement>("telegraph-danger-pip");
        telegraphBarFill = root.Q<VisualElement>("telegraph-bar-fill");
    }

    #endregion

    #region Public API

    /// <summary>
    /// Displays the telegraph panel and populates it with the incoming attack data.
    /// </summary>
    /// <param name="enemyName">Name of the attacking enemy; displayed in uppercase.</param>
    /// <param name="attackName">Human-readable name of the incoming attack.</param>
    /// <param name="duration">Total telegraph window in seconds.</param>
    /// <param name="danger">Severity of the attack; controls pip and bar colour.</param>
    public void Show(string enemyName, string attackName, float duration, TelegraphDanger danger)
    {
        if (telegraphContainer == null) return;

        totalDuration = duration;
        remainingTime = duration;
        currentDanger = danger;

        if (telegraphEnemyName != null) telegraphEnemyName.text = enemyName.ToUpper();
        if (telegraphAttackName != null) telegraphAttackName.text = $"Incoming: {attackName}";

        ApplyDanger(danger);
        SetProgress(1f);

        telegraphContainer.AddToClassList("telegraph-visible");
    }

    /// <summary>
    /// Hides the telegraph panel immediately regardless of remaining time.
    /// Call when the attack resolves, is interrupted, or the enemy dies.
    /// </summary>
    public void Hide()
    {
        if (telegraphContainer == null) return;
        telegraphContainer.RemoveFromClassList("telegraph-visible");
    }

    /// <summary>
    /// Advances the telegraph countdown by <paramref name="deltaTime"/> seconds
    /// and updates the timer label and progress bar. Hides the panel when time expires.
    /// Call once per frame from the owning game system.
    /// </summary>
    /// <param name="deltaTime">Elapsed time since the last frame, typically <c>Time.deltaTime</c>.</param>
    public void Tick(float deltaTime)
    {
        if (totalDuration <= 0f) return;

        remainingTime = Mathf.Max(0f, remainingTime - deltaTime);

        if (telegraphTimer != null)
            telegraphTimer.text = $"{remainingTime:0.0}s";

        SetProgress(remainingTime / totalDuration);

        if (remainingTime <= 0f) Hide();
    }

    /// <summary>
    /// Sets the progress bar fill directly as a normalised fraction.
    /// Use instead of <see cref="Tick"/> when managing telegraph timing externally.
    /// </summary>
    /// <param name="fraction">
    /// Fill amount from 0–1 where 1 is full (attack not yet started) and 0 is empty (attack landing).
    /// Values outside this range are clamped.
    /// </param>
    public void SetProgress(float fraction)
    {
        if (telegraphBarFill == null) return;
        telegraphBarFill.style.width = new Length(Mathf.Clamp01(fraction) * 100f, LengthUnit.Percent);
    }

    #endregion

    #region Private Helpers

    /// <summary>
    /// Swaps the danger USS classes on the pip and bar fill to reflect
    /// the severity of the incoming attack.
    /// </summary>
    /// <param name="danger">The danger level to apply.</param>
    private void ApplyDanger(TelegraphDanger danger)
    {
        string[] dangerClasses = { "telegraph-danger-low", "telegraph-danger-medium", "telegraph-danger-high" };

        foreach (string cls in dangerClasses)
        {
            telegraphDangerPip?.RemoveFromClassList(cls);
            telegraphBarFill?.RemoveFromClassList(cls);
        }

        string dangerClass = danger switch
        {
            TelegraphDanger.Medium => "telegraph-danger-medium",
            TelegraphDanger.High => "telegraph-danger-high",
            _ => "telegraph-danger-low",
        };

        telegraphDangerPip?.AddToClassList(dangerClass);
        telegraphBarFill?.AddToClassList(dangerClass);
    }

    #endregion
}