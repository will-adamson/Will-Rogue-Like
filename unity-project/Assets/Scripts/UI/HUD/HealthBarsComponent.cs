using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
/// HUD component that manages all player stat displays: HP, stamina, and experience bars,
/// numeric stat labels, character sprite, class identity, and class-specific stat rows.
/// </summary>
/// <remarks>
/// Implements <see cref="IHUDComponent"/>. Call <see cref="Init"/> once after the
/// UI Document layout is ready, then drive updates via <see cref="Refresh"/> each frame
/// or <see cref="SetCharacter"/> when the player's class changes.
/// Class-specific stats are built dynamically via <see cref="BuildClassStats"/> using
/// a type-switch on <see cref="PlayerData"/> subtypes.
/// </remarks>
public class HealthBarsComponent : IHUDComponent
{
    #region Cached UI Elements

    /// <summary>Fill element whose width percentage represents current HP.</summary>
    private VisualElement barHpFill;

    /// <summary>Fill element whose width percentage represents current stamina.</summary>
    private VisualElement barStaFill;

    /// <summary>Fill element whose width percentage represents current experience progress.</summary>
    private VisualElement barExpFill;

    /// <summary>Label displaying current/max HP as integers.</summary>
    private Label labelHp;

    /// <summary>Label displaying current/max stamina as integers.</summary>
    private Label labelSta;

    /// <summary>Label displaying the player's current level.</summary>
    private Label labelLevel;

    /// <summary>Label displaying the player's attack stat.</summary>
    private Label labelAtk;

    /// <summary>Label displaying the player's defence stat.</summary>
    private Label labelDef;

    /// <summary>Label displaying the player's speed stat.</summary>
    private Label labelSpd;

    /// <summary>Label displaying the player's critical hit chance as a percentage.</summary>
    private Label labelCrit;

    /// <summary>Label displaying the player's critical hit damage multiplier.</summary>
    private Label labelCritMul;

    /// <summary>Label displaying the player's class name in uppercase.</summary>
    private Label labelClass;

    /// <summary>Label displaying the player's archetype description (e.g. "Melee - Tank").</summary>
    private Label labelType;

    /// <summary>Container whose background image is set to the player's class sprite.</summary>
    private VisualElement charSprite;

    /// <summary>Container into which class-specific stat rows are dynamically added.</summary>
    private VisualElement classStats;

    #endregion

    #region IHUDComponent

    /// <summary>
    /// Resolves and caches all UI element references from <paramref name="root"/>.
    /// Must be called before any other method.
    /// </summary>
    /// <param name="root">Root <see cref="VisualElement"/> of the HUD UI Document.</param>
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

    #endregion

    #region Public API

    /// <summary>
    /// Populates static character information: class name, archetype label, sprite,
    /// and class-specific stat rows. Call once when a character is selected or loaded.
    /// </summary>
    /// <param name="data">The <see cref="PlayerData"/> asset for the chosen class.</param>
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

    /// <summary>
    /// Updates all dynamic HUD values from the provided <see cref="PlayerStats"/> snapshot.
    /// Intended to be called each frame or whenever stats change.
    /// </summary>
    /// <param name="stats">Current runtime stats of the player.</param>
    public void Refresh(PlayerStats stats)
    {
        SetHp(stats.CurrentHp, stats.MaxHp);
        SetStamina(stats.CurrentSta, stats.MaxSta);
        SetExp(stats.CurrentExp, stats.MaxExp, stats.Level);
        SetStats(stats.Damage, stats.Defence, stats.Speed, stats.CritChance, stats.CritMul);
    }

    /// <summary>
    /// Updates the HP bar fill and numeric label.
    /// </summary>
    /// <param name="current">Current HP value.</param>
    /// <param name="max">Maximum HP value.</param>
    public void SetHp(float current, float max)
    {
        if (labelHp == null) return;
        labelHp.text = $"{Mathf.RoundToInt(current)}/{Mathf.RoundToInt(max)}";
        float fillPercent = Mathf.Clamp01(current / max) * 100f;
        barHpFill.style.width = new Length(fillPercent, LengthUnit.Percent);
    }

    /// <summary>
    /// Updates the stamina bar fill and numeric label.
    /// </summary>
    /// <param name="current">Current stamina value.</param>
    /// <param name="max">Maximum stamina value.</param>
    public void SetStamina(float current, float max)
    {
        if (labelSta == null) return;
        labelSta.text = $"{Mathf.RoundToInt(current)}/{Mathf.RoundToInt(max)}";
        float fillPercent = Mathf.Clamp01(current / max) * 100f;
        barStaFill.style.width = new Length(fillPercent, LengthUnit.Percent);
    }

    /// <summary>
    /// Updates the experience bar fill and level label.
    /// </summary>
    /// <param name="current">Current experience points accumulated toward the next level.</param>
    /// <param name="max">Experience points required to reach the next level.</param>
    /// <param name="level">The player's current level, shown in the label.</param>
    public void SetExp(float current, float max, int level)
    {
        if (labelLevel == null) return;
        labelLevel.text = $"Lvl {level}";
        float fillPercent = Mathf.Clamp01(current / max) * 100f;
        barExpFill.style.width = new Length(fillPercent, LengthUnit.Percent);
    }

    /// <summary>
    /// Updates all core combat stat labels (attack, defence, speed, crit chance, crit multiplier).
    /// </summary>
    /// <param name="atk">Attack damage value.</param>
    /// <param name="def">Defence value.</param>
    /// <param name="spd">Movement/attack speed value.</param>
    /// <param name="crit">Critical hit chance as a fraction (0–1); displayed as a percentage.</param>
    /// <param name="critMul">Critical hit damage multiplier; displayed with an "x" prefix.</param>
    public void SetStats(float atk, float def, float spd, float crit, float critMul)
    {
        if (labelAtk == null) return;
        labelAtk.text = Mathf.RoundToInt(atk).ToString();
        labelDef.text = Mathf.RoundToInt(def).ToString();
        labelSpd.text = $"{spd:0.#}";
        labelCrit.text = $"{Mathf.RoundToInt(crit * 100f)}%";
        labelCritMul.text = $"x{critMul:0.##}";
    }

    #endregion

    #region Private Helpers

    /// <summary>
    /// Clears <see cref="classStats"/> and rebuilds it with rows specific to the
    /// player's class subtype. Each row is created via <see cref="AddClassStat"/>.
    /// </summary>
    /// <param name="data">
    /// The <see cref="PlayerData"/> subtype determining which rows are generated.
    /// Supports <see cref="KnightData"/>, <see cref="RogueData"/>, <see cref="MageData"/>,
    /// and <see cref="ArcherData"/>.
    /// </param>
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

    /// <summary>
    /// Creates a key/value row element and appends it to <see cref="classStats"/>.
    /// Applies USS classes <c>class-stat-row</c>, <c>class-stat-key</c>, and <c>class-stat-val</c>
    /// for styling.
    /// </summary>
    /// <param name="key">The stat name displayed on the left of the row.</param>
    /// <param name="value">The formatted stat value displayed on the right of the row.</param>
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

    /// <summary>
    /// Returns a human-readable archetype description for the given <see cref="PlayerData"/> subtype.
    /// </summary>
    /// <param name="data">The class data to classify.</param>
    /// <returns>A string such as "Melee - Tank" or "Ranged - Caster", or "Unknown" for unrecognised types.</returns>
    private string GetClassType(PlayerData data) => data switch
    {
        KnightData => "Melee - Tank",
        RogueData => "Melee - Assassin",
        MageData => "Ranged - Caster",
        ArcherData => "Ranged - Marksman",
        _ => "Unknown"
    };

    #endregion
}