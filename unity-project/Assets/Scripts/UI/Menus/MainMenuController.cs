using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class MainMenuController : MonoBehaviour
{
    [Header("UI Documents")]
    [SerializeField] private UIDocument mainMenuDocument;
    [SerializeField] private UIDocument characterSelectDocument;
    [SerializeField] private UIDocument settingsDocument;

    [Header("Scenes")]
    [SerializeField] private string gameSceneName = "Game";

    [Header("Class Data")]
    [SerializeField] private KnightData knightData;
    [SerializeField] private RogueData rogueData;
    [SerializeField] private MageData mageData;
    [SerializeField] private ArcherData archerData;

    private Button btnNewRun;
    private Button btnContinue;
    private Button btnSettings;
    private Button btnQuit;
    private Button btnCharBack;
    private Button btnCharStart;
    private Button btnSettingsBack;
    private Button btnSettingsApply;

    private Slider sliderMaster;
    private Slider sliderMusic;
    private Slider sliderSfx;

    private Label labelMaster;
    private Label labelMusic;
    private Label labelSfx;

    private Toggle toggleFullscreen;
    private Toggle toggleColorblind;
    private Toggle toggleScreenshake;

    private PlayerData[] classData;

    private readonly string[] cardNames =
    {
        "card-knight",
        "card-rogue",
        "card-mage",
        "card-archer"
    };

    private readonly string[] classNames =
    {
        "Knight",
        "Rogue",
        "Mage",
        "Archer"
    };

    private int selectedIndex = 0;

    private void Awake()
    {
        classData = new PlayerData[]
        {
            knightData,
            rogueData,
            mageData,
            archerData
        };

        SetupMainMenu();
        SetupCharacterSelect();
        SetupSettings();

        mainMenuDocument.sortingOrder = 10;
        characterSelectDocument.sortingOrder = 11;
        settingsDocument.sortingOrder = 11;

        characterSelectDocument.rootVisualElement.style.display = DisplayStyle.None;
        settingsDocument.rootVisualElement.style.display = DisplayStyle.None;

        LoadSettings();
    }

    private void OnDestroy()
    {
        btnNewRun.clicked -= OpenCharacterSelect;
        btnContinue.clicked -= ContinueRun;
        btnSettings.clicked -= OpenSettings;
        btnQuit.clicked -= QuitGame;

        btnCharBack.clicked -= CloseCharacterSelect;
        btnCharStart.clicked -= StartRun;

        btnSettingsBack.clicked -= CloseSettings;
        btnSettingsApply.clicked -= ApplySettings;
    }

    private void SetupMainMenu()
    {
        VisualElement root = mainMenuDocument.rootVisualElement;

        btnNewRun = root.Q<Button>("btn-new-run");
        btnContinue = root.Q<Button>("btn-continue");
        btnSettings = root.Q<Button>("btn-settings");
        btnQuit = root.Q<Button>("btn-quit");

        btnNewRun.clicked += OpenCharacterSelect;
        btnContinue.clicked += ContinueRun;
        btnSettings.clicked += OpenSettings;
        btnQuit.clicked += QuitGame;
    }

    private void ContinueRun()
    {
        Debug.Log("Continue run");
    }

    private void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    private void SetupCharacterSelect()
    {
        VisualElement root = characterSelectDocument.rootVisualElement;

        btnCharBack = root.Q<Button>("btn-back");
        btnCharStart = root.Q<Button>("btn-start");

        btnCharBack.clicked += CloseCharacterSelect;
        btnCharStart.clicked += StartRun;

        for (int i = 0; i < cardNames.Length; i++)
        {
            if (classData[i] != null)
                PopulateCard(root, cardNames[i], classData[i], classNames[i]);
        }

        for (int i = 0; i < cardNames.Length; i++)
        {
            int index = i;
            root.Q<VisualElement>(cardNames[i])?.RegisterCallback<ClickEvent>(_ => SelectClass(index));
        }

        SelectClass(0);
    }

    private void OpenCharacterSelect()
    {
        mainMenuDocument.rootVisualElement.style.display = DisplayStyle.None;
        characterSelectDocument.rootVisualElement.style.display = DisplayStyle.Flex;
    }

    private void CloseCharacterSelect()
    {
        characterSelectDocument.rootVisualElement.style.display = DisplayStyle.None;
        mainMenuDocument.rootVisualElement.style.display = DisplayStyle.Flex;
    }

    private void SelectClass(int index)
    {
        selectedIndex = index;
        VisualElement root = characterSelectDocument.rootVisualElement;

        for (int i = 0; i < cardNames.Length; i++)
        {
            VisualElement card = root.Q<VisualElement>(cardNames[i]);

            if (card == null)
                continue;

            if (i == index)
                card.AddToClassList("card-selected");
            else
                card.RemoveFromClassList("card-selected");
        }

        if (classData[index] != null)
            root.Q<Label>("info-passive").text = GetPassiveText(classData[index]);
    }

    private void StartRun()
    {
        if (classData[selectedIndex] == null)
            return;

        PlayerData selected = classData[selectedIndex];

        GameSession.SetClass(selected);

        PlayerPrefs.SetString("selectedClass", classNames[selectedIndex]);
        PlayerPrefs.SetString("hasSave", "true");
        PlayerPrefs.Save();

        SceneManager.LoadScene(gameSceneName);
    }

    private void PopulateCard(
        VisualElement root,
        string cardName,
        PlayerData data,
        string displayName)
    {
        VisualElement card = root.Q<VisualElement>(cardName);

        if (card == null)
            return;

        VisualElement spriteEl = card.Q<VisualElement>("card-sprite");

        if (spriteEl != null && data.sprite != null)
            spriteEl.style.backgroundImage = new StyleBackground(data.sprite);

        Label nameLabel = card.Q<Label>("card-name");

        if (nameLabel != null)
            nameLabel.text = displayName.ToUpper();

        Label descLabel = card.Q<Label>("card-desc");

        if (descLabel != null)
            descLabel.text = $"HP: {data.health}  ATK: {data.damage}  SPD: {data.speed}";

        Label passiveLabel = card.Q<Label>("card-passive");

        if (passiveLabel != null)
            passiveLabel.text = GetPassiveText(data);

        SetStatBar(card, "stat-hp-fill", data.health, 200f);
        SetStatBar(card, "stat-atk-fill", data.damage, 50f);
        SetStatBar(card, "stat-spd-fill", data.speed, 10f);
    }

    private void SetStatBar(
        VisualElement card,
        string fillName,
        float value,
        float max)
    {
        VisualElement fill = card.Q<VisualElement>(fillName);

        if (fill == null)
            return;

        fill.style.width =
            Length.Percent(Mathf.Clamp01(value / max) * 100f);
    }

    private string GetPassiveText(PlayerData data) => data switch
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

    private void SetupSettings()
    {
        VisualElement root = settingsDocument.rootVisualElement;

        btnSettingsBack = root.Q<Button>("btn-back");
        btnSettingsApply = root.Q<Button>("btn-apply");

        btnSettingsBack.clicked += CloseSettings;
        btnSettingsApply.clicked += ApplySettings;

        sliderMaster = root.Q<Slider>("slider-master");
        sliderMusic = root.Q<Slider>("slider-music");
        sliderSfx = root.Q<Slider>("slider-sfx");

        labelMaster = root.Q<Label>("label-master");
        labelMusic = root.Q<Label>("label-music");
        labelSfx = root.Q<Label>("label-sfx");

        toggleFullscreen = root.Q<Toggle>("toggle-fullscreen");
        toggleColorblind = root.Q<Toggle>("toggle-colorblind");
        toggleScreenshake = root.Q<Toggle>("toggle-screenshake");

        sliderMaster.RegisterValueChangedCallback(
            e => labelMaster.text = Mathf.RoundToInt(e.newValue).ToString());

        sliderMusic.RegisterValueChangedCallback(
            e => labelMusic.text = Mathf.RoundToInt(e.newValue).ToString());

        sliderSfx.RegisterValueChangedCallback(
            e => labelSfx.text = Mathf.RoundToInt(e.newValue).ToString());
    }

    private void OpenSettings()
    {
        mainMenuDocument.rootVisualElement.style.display = DisplayStyle.None;
        settingsDocument.rootVisualElement.style.display = DisplayStyle.Flex;
    }

    private void CloseSettings()
    {
        settingsDocument.rootVisualElement.style.display = DisplayStyle.None;
        mainMenuDocument.rootVisualElement.style.display = DisplayStyle.Flex;
    }

    private void ApplySettings()
    {
        PlayerPrefs.SetFloat("vol-master", sliderMaster.value);
        PlayerPrefs.SetFloat("vol-music", sliderMusic.value);
        PlayerPrefs.SetFloat("vol-sfx", sliderSfx.value);

        PlayerPrefs.SetInt("fullscreen", toggleFullscreen.value ? 1 : 0);
        PlayerPrefs.SetInt("colorblind", toggleColorblind.value ? 1 : 0);
        PlayerPrefs.SetInt("screenshake", toggleScreenshake.value ? 1 : 0);

        PlayerPrefs.Save();

        Screen.fullScreen = toggleFullscreen.value;

        CloseSettings();
    }

    private void LoadSettings()
    {
        sliderMaster.value = PlayerPrefs.GetFloat("vol-master", 80f);
        sliderMusic.value = PlayerPrefs.GetFloat("vol-music", 70f);
        sliderSfx.value = PlayerPrefs.GetFloat("vol-sfx", 90f);

        toggleFullscreen.value = PlayerPrefs.GetInt("fullscreen", 1) == 1;
        toggleColorblind.value = PlayerPrefs.GetInt("colorblind", 0) == 1;
        toggleScreenshake.value = PlayerPrefs.GetInt("screenshake", 1) == 1;

        labelMaster.text = Mathf.RoundToInt(sliderMaster.value).ToString();
        labelMusic.text = Mathf.RoundToInt(sliderMusic.value).ToString();
        labelSfx.text = Mathf.RoundToInt(sliderSfx.value).ToString();
    }
}
