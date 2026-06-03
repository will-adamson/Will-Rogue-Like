using UnityEngine;
using UnityEngine.UIElements;

public class HealthBarsComponent : IHUDComponent
{
    private VisualElement barHpFill;
    private VisualElement barStaFill;
    private VisualElement barExpFill;
    private Label labelHp;
    private Label labelSta;
    private Label labelLevel;
    private Label labelAtk;
    private Label labelDef;
    private Label labelSpd;
    private Label labelCrit;
    private Label labelCritMul;
    private Label labelClass;
    private Label labelType;
    private VisualElement charSprite;
    private VisualElement classStats;

    public void Init(VisualElement root)
    {
        barHpFill = root.Q<VisualElement>("bar-hp-fill");
        barStaFill = root.Q<VisualElement>("bar-sta-fill");
        barExpFill = root.Q<VisualElement>("bar-exp-fill");
        labelHp = root.Q<Label>("label-hp");
        labelSta = root.Q<Label>("label-sta");
        labelLevel = root.Q<Label>("label-level");
        labelAtk = root.Q<Label>("label-atk");
        labelDef = root.Q<Label>("label-def");
        labelSpd = root.Q<Label>("label-spd");
        labelCrit = root.Q<Label>("label-crit");
        labelCritMul = root.Q<Label>("label-crit-mul");
        labelClass = root.Q<Label>("label-class");
        labelType = root.Q<Label>("label-type");
        charSprite = root.Q<VisualElement>("char-sprite");
        classStats = root.Q<VisualElement>("class-stats");
    }

    public void SetCharacter(PlayerData data)
    {
        if (labelClass != null)
            labelClass.text = data.name.ToUpper();

        if (labelType != null)
            labelType.text = GetClassType(data);

        if (charSprite != null && data.sprite != null)
            charSprite.style.backgroundImage = new StyleBackground(data.sprite);

        BuildClassStats(data);
    }

    public void Refresh(PlayerStats stats)
    {
        SetHp(stats.CurrentHp, stats.MaxHp);
        SetStamina(stats.CurrentSta, stats.MaxSta);
        SetExp(stats.CurrentExp, stats.MaxExp, stats.Level);
        SetStats(stats.Damage, stats.Defence, stats.Speed, stats.CritChance, stats.CritMul);
    }

    public void SetHp(float current, float max)
    {
        if (labelHp == null) return;
        labelHp.text = $"{Mathf.RoundToInt(current)}/{Mathf.RoundToInt(max)}";
        float fillPercent = Mathf.Clamp01(current / max) * 100f;
        barHpFill.style.width = new Length(fillPercent, LengthUnit.Percent);
    }

    public void SetStamina(float current, float max)
    {
        if (labelSta == null) return;
        labelSta.text = $"{Mathf.RoundToInt(current)}/{Mathf.RoundToInt(max)}";
        float fillPercent = Mathf.Clamp01(current / max) * 100f;
        barStaFill.style.width = new Length(fillPercent, LengthUnit.Percent);
    }

    public void SetExp(float current, float max, int level)
    {
        if (labelLevel == null) return;
        labelLevel.text = $"Lvl {level}";
        float fillPercent = Mathf.Clamp01(current / max) * 100f;
        barExpFill.style.width = new Length(fillPercent, LengthUnit.Percent);
    }

    public void SetStats(float atk, float def, float spd, float crit, float critMul)
    {
        if (labelAtk == null) return;
        labelAtk.text = Mathf.RoundToInt(atk).ToString();
        labelDef.text = Mathf.RoundToInt(def).ToString();
        labelSpd.text = $"{spd:0.#}";
        labelCrit.text = $"{Mathf.RoundToInt(crit * 100f)}%";
        labelCritMul.text = $"x{critMul:0.##}";
    }

    private void BuildClassStats(PlayerData data)
    {
        if (classStats == null) return;
        classStats.Clear();

        switch (data)
        {
            case KnightData k:
                AddClassStat("Block Reduction", $"{k.blockDamageReduction * 100f:0}%");
                AddClassStat("Charge Speed", $"{k.chargeSpeed:0.#}");
                AddClassStat("Attack Range", $"{k.attackRange:0.#}");
                AddClassStat("Knockback", $"{k.knockbackForce:0.#}");
                break;

            case RogueData r:
                AddClassStat("Dodge Chance", $"{r.dodgeChance * 100f:0}%");
                AddClassStat("Backstab Mul", $"x{r.backstabMultiplier:0.#}");
                AddClassStat("Dash Speed", $"{r.dashSpeed:0.#}");
                AddClassStat("Dash Cooldown", $"{r.dashCooldown:0.#}s");
                AddClassStat("Attack Range", $"{r.attackRange:0.#}");
                break;

            case MageData m:
                AddClassStat("Cast Cooldown", $"{m.castCooldown:0.##}s");
                AddClassStat("Spell Range", $"{m.spellRange:0.#}");
                AddClassStat("Spell Speed", $"{m.spellSpeed:0.#}");
                AddClassStat("Projectiles", $"{m.projectilesPerCast}");
                AddClassStat("Spread", $"{m.spreadAngle:0}°");
                break;

            case ArcherData a:
                AddClassStat("Arrow Range", $"{a.arrowRange:0.#}");
                AddClassStat("Arrow Speed", $"{a.arrowSpeed:0.#}");
                AddClassStat("Draw Cooldown", $"{a.drawCooldown:0.##}s");
                AddClassStat("Arrows/Shot", $"{a.arrowsPerShot}");
                AddClassStat("Spread", $"{a.spreadAngle:0}°");
                AddClassStat("Charged Mul", $"x{a.chargedShotMultiplier:0.#}");
                break;
        }
    }

    private void AddClassStat(string key, string value)
    {
        VisualElement row = new VisualElement();
        row.AddToClassList("class-stat-row");

        Label keyLabel = new Label { text = key };
        keyLabel.AddToClassList("class-stat-key");

        Label valLabel = new Label { text = value };
        valLabel.AddToClassList("class-stat-val");

        row.Add(keyLabel);
        row.Add(valLabel);
        classStats.Add(row);
    }

    private string GetClassType(PlayerData data) => data switch
    {
        KnightData => "Melee — Tank",
        RogueData => "Melee — Assassin",
        MageData => "Ranged — Caster",
        ArcherData => "Ranged — Marksman",
        _ => "Unknown"
    };
}
