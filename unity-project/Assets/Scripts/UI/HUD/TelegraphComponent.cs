using UnityEngine;
using UnityEngine.UIElements;

public class TelegraphComponent : IHUDComponent
{
    private VisualElement telegraphContainer;
    private Label telegraphEnemyName;
    private Label telegraphAttackName;
    private Label telegraphTimer;
    private VisualElement telegraphDangerPip;
    private VisualElement telegraphBarFill;

    private float totalDuration;
    private float remainingTime;

    private TelegraphDanger currentDanger;

    public void Init(VisualElement root)
    {
        telegraphContainer = root.Q<VisualElement>("telegraph-container");
        telegraphEnemyName = root.Q<Label>("telegraph-enemy-name");
        telegraphAttackName = root.Q<Label>("telegraph-attack-name");
        telegraphTimer = root.Q<Label>("telegraph-timer");
        telegraphDangerPip = root.Q<VisualElement>("telegraph-danger-pip");
        telegraphBarFill = root.Q<VisualElement>("telegraph-bar-fill");
    }

    public void Show(
        string enemyName,
        string attackName,
        float duration,
        TelegraphDanger danger)
    {
        if (telegraphContainer == null)
            return;

        totalDuration = duration;
        remainingTime = duration;
        currentDanger = danger;

        if (telegraphEnemyName != null)
            telegraphEnemyName.text = enemyName.ToUpper();

        if (telegraphAttackName != null)
            telegraphAttackName.text = $"Incoming: {attackName}";

        ApplyDanger(danger);
        SetProgress(1f);

        telegraphContainer.AddToClassList("telegraph-visible");
    }

    public void Hide()
    {
        if (telegraphContainer == null)
            return;

        telegraphContainer.RemoveFromClassList("telegraph-visible");
    }

    public void Tick(float deltaTime)
    {
        if (totalDuration <= 0f)
            return;

        remainingTime = Mathf.Max(0f, remainingTime - deltaTime);

        if (telegraphTimer != null)
            telegraphTimer.text = $"{remainingTime:0.0}s";

        SetProgress(remainingTime / totalDuration);

        if (remainingTime <= 0f)
            Hide();
    }

    public void SetProgress(float fraction)
    {
        if (telegraphBarFill == null)
            return;

        telegraphBarFill.style.width =
            new Length(Mathf.Clamp01(fraction) * 100f, LengthUnit.Percent);
    }

    private void ApplyDanger(TelegraphDanger danger)
    {
        string[] dangerClasses =
        {
            "telegraph-danger-low",
            "telegraph-danger-medium",
            "telegraph-danger-high"
        };

        foreach (string cls in dangerClasses)
        {
            telegraphDangerPip?.RemoveFromClassList(cls);
            telegraphBarFill?.RemoveFromClassList(cls);
        }

        string dangerClass = danger switch
        {
            TelegraphDanger.Medium => "telegraph-danger-medium",
            TelegraphDanger.High => "telegraph-danger-high",
            _ => "telegraph-danger-low"
        };

        telegraphDangerPip?.AddToClassList(dangerClass);
        telegraphBarFill?.AddToClassList(dangerClass);
    }
}