using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(UIDocument))]
public class CharacterSelectMenuController : MonoBehaviour
{
    [Header("Scenes")]
    [SerializeField] private string mainMenuScene = "Main Menu";
    [SerializeField] private string gameScene = "Game";

    [Header("Class Data")]
    [SerializeField] private KnightData knightData;
    [SerializeField] private RogueData rogueData;
    [SerializeField] private MageData mageData;
    [SerializeField] private ArcherData archerData;

    private PlayerData[] classData;
    private readonly string[] cardNames = { "card-knight", "card-rogue", "card-mage", "card-archer" };
    private readonly string[] classNames = { "Knight", "Rogue", "Mage", "Archer" };

    private int selectedIndex = 0;
    private VisualElement root;

    private void OnEnable()
    {
        root = GetComponent<UIDocument>().rootVisualElement;

        classData = new PlayerData[] { knightData, rogueData, mageData, archerData };

        for (int i = 0; i < cardNames.Length; i++)
        {
            if (classData[i] != null)
                PopulateCard(cardNames[i], classData[i], classNames[i]);
        }

        for (int i = 0; i < cardNames.Length; i++)
        {
            int index = i;
            VisualElement card = root.Q<VisualElement>(cardNames[i]);
            card?.RegisterCallback<ClickEvent>(_ => SelectClass(index));
        }

        root.Q<Button>("btn-back").clicked += () => SceneManager.LoadScene(mainMenuScene);
        root.Q<Button>("btn-start").clicked += StartRun;

        SelectClass(0);
    }

    private void PopulateCard(string cardName, PlayerData data, string displayName)
    {
        VisualElement card = root.Q<VisualElement>(cardName);
        if (card == null) return;

        VisualElement spriteEl = card.Q<VisualElement>("card-sprite");
        if (spriteEl != null && data.sprite != null)
            spriteEl.style.backgroundImage = new StyleBackground(data.sprite);

        Label nameLabel = card.Q<Label>("card-name");
        if (nameLabel != null) nameLabel.text = displayName.ToUpper();

        Label descLabel = card.Q<Label>("card-desc");
        if (descLabel != null)
            descLabel.text = $"HP: {data.health}  ATK: {data.damage}  SPD: {data.speed}";

        Label passiveLabel = card.Q<Label>("card-passive");
        if (passiveLabel != null) passiveLabel.text = GetPassiveText(data);

        SetStatBar(card, "stat-hp-fill", data.health, 200f);
        SetStatBar(card, "stat-atk-fill", data.damage, 50f);
        SetStatBar(card, "stat-spd-fill", data.speed, 10f);
    }

    private string GetPassiveText(PlayerData data)
    {
        return data switch
        {
            KnightData k =>
                $"Block Reduction: {k.blockDamageReduction * 100f:0}%  |  Charge Speed: {k.chargeSpeed}",
            RogueData r =>
                $"Dodge: {r.dodgeChance * 100f:0}%  |  Backstab: x{r.backstabMultiplier}  |  Dash Speed: {r.dashSpeed}  |  Dash CD: {r.dashCooldown}s",
            MageData m =>
                $"Cast CD: {m.castCooldown}s  |  Range: {m.spellRange}  |  Speed: {m.spellSpeed}  |  Projectiles: {m.projectilesPerCast}  |  Spread: {m.spreadAngle}°",
            ArcherData a =>
                $"Range: {a.arrowRange}  |  Speed: {a.arrowSpeed}  |  Draw CD: {a.drawCooldown}s  |  Arrows: {a.arrowsPerShot}  |  Spread: {a.spreadAngle}°  |  Charged: x{a.chargedShotMultiplier}",
            _ => ""
        };
    }

    private void SetStatBar(VisualElement card, string fillName, float value, float max)
    {
        VisualElement fill = card.Q<VisualElement>(fillName);
        if (fill == null) return;
        float pct = Mathf.Clamp01(value / max) * 100f;
        fill.style.width = Length.Percent(pct);
    }

    private void SelectClass(int index)
    {
        selectedIndex = index;

        for (int i = 0; i < cardNames.Length; i++)
        {
            VisualElement card = root.Q<VisualElement>(cardNames[i]);
            if (card == null) continue;
            if (i == index) card.AddToClassList("card-selected");
            else card.RemoveFromClassList("card-selected");
        }

        if (classData[index] != null)
            root.Q<Label>("info-passive").text = GetPassiveText(classData[index]);
    }

    private void StartRun()
    {
        if (classData[selectedIndex] == null) return;

        PlayerData selected = classData[selectedIndex];

        PlayerPrefs.SetString("selectedClass", classNames[selectedIndex]);
        PlayerPrefs.SetFloat("startingHp", selected.health);
        PlayerPrefs.SetFloat("startingAtk", selected.damage);
        PlayerPrefs.SetFloat("startingSpd", selected.speed);
        PlayerPrefs.SetString("hasSave", "true");
        PlayerPrefs.Save();

        SceneManager.LoadScene(gameScene);
    }
}