using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class StatusEffectTrayComponent : IHUDComponent
{
    private VisualElement buffTray;
    private VisualElement debuffTray;

    private readonly Dictionary<string, TrayEntry> entries = new();

    private class TrayEntry
    {
        public VisualElement Root;
        public VisualElement Icon;
        public Label Timer;
        public Label Stacks;
        public float Duration;
        public float Remaining;
        public int StackCount;
    }

    public void Init(VisualElement root)
    {
        buffTray = root.Q<VisualElement>("buff-tray");
        debuffTray = root.Q<VisualElement>("debuff-tray");
    }

    public void AddEffect(string id, Sprite icon, float duration, int stacks, bool isBuff)
    {
        if (entries.TryGetValue(id, out TrayEntry existing))
        {
            existing.Remaining = duration;
            existing.StackCount = stacks;
            existing.Stacks.text = stacks > 1 ? stacks.ToString() : "";
            return;
        }

        VisualElement slot = new();
        slot.AddToClassList("tray-slot");

        if (!isBuff)
            slot.AddToClassList("tray-slot-debuff");

        VisualElement iconEl = new();
        iconEl.AddToClassList("tray-icon");
        if (icon != null)
            iconEl.style.backgroundImage = new StyleBackground(icon);

        Label timerLabel = new();
        timerLabel.AddToClassList("tray-timer");
        timerLabel.text = FormatTime(duration);

        Label stackLabel = new();
        stackLabel.AddToClassList("tray-stacks");
        stackLabel.text = stacks > 1 ? stacks.ToString() : "";

        slot.Add(iconEl);
        slot.Add(timerLabel);
        slot.Add(stackLabel);

        VisualElement tray = isBuff ? buffTray : debuffTray;
        tray?.Add(slot);

        entries[id] = new TrayEntry
        {
            Root = slot,
            Icon = iconEl,
            Timer = timerLabel,
            Stacks = stackLabel,
            Duration = duration,
            Remaining = duration,
            StackCount = stacks
        };
    }

    public void RemoveEffect(string id)
    {
        if (!entries.TryGetValue(id, out TrayEntry entry)) return;
        entry.Root.RemoveFromHierarchy();
        entries.Remove(id);
    }

    public void Tick(float deltaTime)
    {
        List<string> expired = null;

        foreach (var kvp in entries)
        {
            TrayEntry e = kvp.Value;
            e.Remaining = Mathf.Max(0f, e.Remaining - deltaTime);
            e.Timer.text = FormatTime(e.Remaining);

            float frac = e.Duration > 0f ? e.Remaining / e.Duration : 0f;
            e.Icon.style.opacity = Mathf.Lerp(0.4f, 1f, frac);

            if (e.Remaining <= 0f)
            {
                expired ??= new List<string>();
                expired.Add(kvp.Key);
            }
        }

        if (expired == null) return;
        foreach (string id in expired)
            RemoveEffect(id);
    }

    private static string FormatTime(float t) =>
        t >= 10f ? $"{Mathf.CeilToInt(t)}s" : $"{t:0.0}s";
}