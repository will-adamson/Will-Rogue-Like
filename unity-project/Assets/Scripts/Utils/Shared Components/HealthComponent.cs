using System;
using UnityEngine;

public class HealthComponent : MonoBehaviour
{
    public float Current { get; private set; }
    public float Max { get; private set; }
    public bool IsDead { get; private set; }

    public event Action<float, float> OnHealthChanged; 
    public event Action OnDeath;

    public void Init(float max)
    {
        Max = max;
        Current = max;
        IsDead = false;
    }

    public float ApplyDamage(float rawAmount, float defence)
    {
        if (IsDead) return 0f;

        float mitigated = Mathf.Max(0f, rawAmount - defence);
        Current = Mathf.Clamp(Current - mitigated, 0f, Max);

        OnHealthChanged?.Invoke(Current, Max);

        if (Current <= 0f && !IsDead)
        {
            IsDead = true;
            OnDeath?.Invoke();
        }

        return mitigated;
    }
}