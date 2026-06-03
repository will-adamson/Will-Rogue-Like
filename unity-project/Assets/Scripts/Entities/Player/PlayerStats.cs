using UnityEngine;

public class PlayerStats
{
    public float CurrentHp { get; private set; }
    public float MaxHp { get; private set; }
    public float CurrentSta { get; private set; }
    public float MaxSta { get; private set; }
    public float CurrentExp { get; private set; }
    public float MaxExp { get; private set; }
    public int Level { get; private set; }

    public float Damage { get; private set; }
    public float Defence { get; private set; }
    public float Speed { get; private set; }
    public float CritChance { get; private set; }
    public float CritMul { get; private set; }

    public event System.Action OnChanged;

    public PlayerStats(PlayerData data)
    {
        MaxHp = data.health;
        CurrentHp = data.health;
        MaxSta = data.stamina;
        CurrentSta = data.stamina;
        MaxExp = data.experienceToNextLevel;
        CurrentExp = 0f;
        Level = 1;

        Damage = data.damage;
        Defence = data.defence;
        Speed = data.speed;
        CritChance = data.critChance;
        CritMul = data.critMultiplier;
    }

    public void ModifyHp(float delta)
    {
        CurrentHp = Mathf.Clamp(CurrentHp + delta, 0f, MaxHp);
        OnChanged?.Invoke();
    }

    public void ModifyStamina(float delta)
    {
        CurrentSta = Mathf.Clamp(CurrentSta + delta, 0f, MaxSta);
        OnChanged?.Invoke();
    }

    public void AddExp(float amount)
    {
        CurrentExp += amount;
        while (CurrentExp >= MaxExp)
        {
            CurrentExp -= MaxExp;
            Level++;
            MaxExp *= 1.2f;
        }
        OnChanged?.Invoke();
    }

    public void ModifyDamage(float delta)
    {
        Damage = Mathf.Max(0f, Damage + delta);
        OnChanged?.Invoke();
    }

    public void ModifyDefence(float delta)
    {
        Defence = Mathf.Max(0f, Defence + delta);
        OnChanged?.Invoke();
    }

    public void ModifySpeed(float delta)
    {
        Speed = Mathf.Max(0f, Speed + delta);
        OnChanged?.Invoke();
    }

    public void ModifyCritChance(float delta)
    {
        CritChance = Mathf.Clamp01(CritChance + delta);
        OnChanged?.Invoke();
    }

    public void ModifyCritMul(float delta)
    {
        CritMul = Mathf.Max(1f, CritMul + delta);
        OnChanged?.Invoke();
    }
}