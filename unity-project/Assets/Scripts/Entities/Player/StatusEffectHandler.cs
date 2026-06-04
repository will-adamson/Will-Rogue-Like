using System.Collections.Generic;
using UnityEngine;

public class StatusEffectHandler : MonoBehaviour, IStatusEffectable
{
    private readonly Dictionary<string, ActiveEffect> active = new Dictionary<string, ActiveEffect>();

    private MoveComponent moveComp;

    private class ActiveEffect
    {
        public StatusEffect Data;
        public float Remaining;
        public int Stacks;
        public float LogTimer;
    }

    private void Awake()
    {
        moveComp = GetComponent<MoveComponent>();
    }

    public void ApplyEffect(StatusEffect effect)
    {
        if (active.TryGetValue(effect.effectName, out ActiveEffect existing))
        {
            existing.Stacks = Mathf.Min(existing.Stacks + 1, effect.maxStacks);

            HUDController.Instance?.StatusTray.AddEffect(
                effect.effectName,
                null,
                existing.Remaining,
                existing.Stacks,
                isBuff: effect.damagePerSecond <= 0f && effect.speedMultiplier >= 1f
            );
            return;
        }

        active[effect.effectName] = new ActiveEffect
        {
            Data = effect,
            Remaining = effect.duration,
            Stacks = 1,
            LogTimer = 0f
        };

        HUDController.Instance?.StatusTray.AddEffect(
            effect.effectName,
            null,
            effect.duration,
            1,
            isBuff: effect.damagePerSecond <= 0f && effect.speedMultiplier >= 1f
        );

        HUDController.Instance?.LogFeed.LogDamage(
            $"{effect.effectName} is affecting you.");
    }

    private void Update()
    {
        if (active.Count == 0) return;

        List<string> expired = null;

        foreach (var kvp in active)
        {
            ActiveEffect e = kvp.Value;
            e.Remaining -= Time.deltaTime;

            if (e.Data.damagePerSecond > 0f)
            {
                if (TryGetComponent(out HealthComponent health) &&
                    TryGetComponent(out PlayerController player))
                {
                    float defence = e.Data.bypassDefence ? 0f : player.Defence;
                    float mitigated = Mathf.Max(0f, e.Data.damagePerSecond * Time.deltaTime - defence);

                    health.ApplyDamage(e.Data.damagePerSecond * Time.deltaTime, defence);
                    player.Stats?.ModifyHp(-mitigated);

                    e.LogTimer -= Time.deltaTime;
                    if (e.LogTimer <= 0f)
                    {
                        e.LogTimer = 1f;
                        if (mitigated > 0f)
                            HUDController.Instance?.LogFeed.LogDamage(
                                $"You took {Mathf.RoundToInt(e.Data.damagePerSecond)} {e.Data.effectName} damage.");
                    }
                }
            }

            if (e.Remaining <= 0f)
            {
                expired ??= new List<string>();
                expired.Add(kvp.Key);
            }
        }

        if (expired != null)
        {
            foreach (string key in expired)
            {
                active.Remove(key);
                HUDController.Instance?.StatusTray.RemoveEffect(key);
            }
        }

        ApplySpeedEffects();
    }

    private float GetDefence()
    {
        PlayerController player = GetComponent<PlayerController>();
        return player != null ? player.Defence : 0f;
    }

    private void ApplySpeedEffects()
    {
        if (moveComp == null) return;

        float multiplier = 1f;
        foreach (var e in active.Values)
            multiplier *= e.Data.speedMultiplier;

        if (multiplier < 1f)
            moveComp.SetSpeed(moveComp.GetBaseSpeed() * multiplier);
        else
            moveComp.ResetSpeed();
    }

    public bool HasEffect(string effectName) => active.ContainsKey(effectName);
}