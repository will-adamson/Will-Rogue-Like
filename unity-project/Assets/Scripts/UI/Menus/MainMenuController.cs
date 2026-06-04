using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

/// <summary>
/// Controls the main menu, character-select, and settings screens via UI Toolkit documents.
/// Manages panel visibility, class card selection, stat-bar population, and settings persistence.
/// </summary>
/// <remarks>
/// Three <see cref="UIDocument"/> assets are layered using <c>sortingOrder</c>:
/// the main menu sits at 10, while character-select and settings overlay at 11.
/// Settings are persisted to <see cref="PlayerPrefs"/> via <see cref="ApplySettings"/>
/// and restored on <see cref="Awake"/> via <see cref="LoadSettings"/>.
/// The selected class is passed to the game scene through <c>GameSession.SetClass</c>.
/// </remarks>
public class MainMenuController : MonoBehaviour
{
    #region Inspector Fields

    [Header("UI Documents")]
    /// <summary>UI Document containing the root main menu panel.</summary>
    [SerializeField] private UIDocument mainMenuDocument;

    /// <summary>UI Document containing the character-selection panel.</summary>
    [SerializeField] private UIDocument characterSelectDocument;

    /// <summary>UI Document containing the settings panel.</summary>
    [SerializeField] private UIDocument settingsDocument;

    [Header("Scenes")]
    /// <summary>Build name of the scene loaded when a run is started.</summary>
    [SerializeField] private string gameSceneName = "Game";

    [Header("Class Data")]
    /// <summary>ScriptableObject data asset for the Knight class.</summary>
    [SerializeField] private KnightData knightData;

    /// <summary>ScriptableObject data asset for the Rogue class.</summary>
    [SerializeField] private RogueData rogueData;

    /// <summary>ScriptableObject data asset for the Mage class.</summary>
    [SerializeField] private MageData mageData;

    /// <summary>ScriptableObject data asset for the Archer class.</summary>
    [SerializeField] private ArcherData archerData;

    #endregion

    #region Cached UI Buttons & Controls

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

    #endregion

    #region Private State

    /// <summary>Ordered array of class data assets mirroring <see cref="cardNames"/>.</summary>
    private PlayerData[] classData;

    /// <summary>UI element names of the four class cards in the character-select panel.</summary>
    private readonly string[] cardNames = { "card-knight", "card-rogue", "card-mage", "card-archer" };

    /// <summary>Display names matching the class order in <see cref="cardNames"/>.</summary>
    private readonly string[] classNames = { "Knight", "Rogue", "Mage", "Archer" };

    /// <summary>Index into <see cref="classData"/> of the currently highlighted class card.</summary>
    private int selectedIndex = 0;

    #endregion

    #region Unity Lifecycle

    /// <summary>
    /// Builds class data array, sets up all three panels, configures sorting orders,
    /// hides overlay panels, and loads persisted settings.
    /// </summary>
    private void Awake()
    {
        classData = new PlayerData[] { knightData, rogueData, mageData, archerData };

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

    /// <summary>
    /// Unsubscribes all button click events to prevent memory leaks on destroy.
    /// </summary>
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

    #endregion

    #region Main Menu Setup & Handlers

    /// <summary>
    /// Queries the main menu document for buttons and subscribes click handlers.
    /// </summary>
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

    /// <summary>
    /// Placeholder for continuing a saved run. Not yet implemented.
    /// </summary>
    private void ContinueRun()
    {
        Debug.Log("Continue run");
    }

    /// <summary>
    /// Quits the application. In the Editor, stops Play mode instead.
    /// </summary>
    private void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    #endregion

    #region Character Select Setup & Handlers

    /// <summary>
    /// Queries the character-select document for buttons and class cards,
    /// populates each card with data, and registers click callbacks.
    /// </summary>
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

    /// <summary>Hides the main menu and shows the character-select panel.</summary>
    private void OpenCharacterSelect()
    {
        mainMenuDocument.rootVisualElement.style.display = DisplayStyle.None;
        characterSelectDocument.rootVisualElement.style.display = DisplayStyle.Flex;
    }

    /// <summary>Hides the character-select panel and shows the main menu.</summary>
    private void CloseCharacterSelect()
    {
        characterSelectDocument.rootVisualElement.style.display = DisplayStyle.None;
        mainMenuDocument.rootVisualElement.style.display = DisplayStyle.Flex;
    }

    /// <summary>
    /// Marks the card at <paramref name="index"/> as selected by toggling the
    /// <c>card-selected</c> USS class, and updates the passive info label.
    /// </summary>
    /// <param name="index">Zero-based index into <see cref="classData"/> and <see cref="cardNames"/>.</param>
    private void SelectClass(int index)
    {
        selectedIndex = index;
        VisualElement root = characterSelectDocument.rootVisualElement;

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

    /// <summary>
    /// Passes the selected class to <c>GameSession</c>, saves the selection to
    /// <see cref="PlayerPrefs"/>, and loads the game scene.
    /// Does nothing if the selected class data asset is null.
    /// </summary>
    private void StartRun()
    {
        if (classData[selectedIndex] == null) return;

        PlayerData selected = classData[selectedIndex];
        GameSession.SetClass(selected);

        PlayerPrefs.SetString("selectedClass", classNames[selectedIndex]);
        PlayerPrefs.SetString("hasSave", "true");
        PlayerPrefs.Save();

        SceneManager.LoadScene(gameSceneName);
    }

    /// <summary>
    /// Fills a class card element with the character sprite, name, stat summary,
    /// passive text, and proportional stat bars.
    /// </summary>
    /// <param name="root">Root of the character-select document.</param>
    /// <param name="cardName">Name of the card container element in the UXML.</param>
    /// <param name="data">Class data providing stat values and sprite.</param>
    /// <param name="displayName">Human-readable class name shown on the card.</param>
    private void PopulateCard(VisualElement root, string cardName, PlayerData data, string displayName)
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

    /// <summary>
    /// Sets the width of a stat bar fill element as a percentage of a given maximum.
    /// </summary>
    /// <param name="card">The card container holding the fill element.</param>
    /// <param name="fillName">Name of the fill <see cref="VisualElement"/> inside the card.</param>
    /// <param name="value">The stat value to represent.</param>
    /// <param name="max">The value that corresponds to 100% fill.</param>
    private void SetStatBar(VisualElement card, string fillName, float value, float max)
    {
        VisualElement fill = card.Q<VisualElement>(fillName);
        if (fill == null) return;
        fill.style.width = Length.Percent(Mathf.Clamp01(value / max) * 100f);
    }

    /// <summary>
    /// Returns a compact, formatted string of class-specific passive stats
    /// for display in the character-select info panel.
    /// </summary>
    /// <param name="data">The class data to summarise.</param>
    /// <returns>A pipe-separated stat string, or an empty string for unknown types.</returns>
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

    #endregion

    #region Settings Setup & Handlers

    /// <summary>
    /// Queries the settings document for all controls and subscribes slider callbacks
    /// that update their adjacent value labels in real time.
    /// </summary>
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

        sliderMaster.RegisterValueChangedCallback(e => labelMaster.text = Mathf.RoundToInt(e.newValue).ToString());
        sliderMusic.RegisterValueChangedCallback(e => labelMusic.text = Mathf.RoundToInt(e.newValue).ToString());
        sliderSfx.RegisterValueChangedCallback(e => labelSfx.text = Mathf.RoundToInt(e.newValue).ToString());
    }

    /// <summary>Hides the main menu and shows the settings panel.</summary>
    private void OpenSettings()
    {
        mainMenuDocument.rootVisualElement.style.display = DisplayStyle.None;
        settingsDocument.rootVisualElement.style.display = DisplayStyle.Flex;
    }

    /// <summary>Hides the settings panel and shows the main menu.</summary>
    private void CloseSettings()
    {
        settingsDocument.rootVisualElement.style.display = DisplayStyle.None;
        mainMenuDocument.rootVisualElement.style.display = DisplayStyle.Flex;
    }

    /// <summary>
    /// Writes all current control values to <see cref="PlayerPrefs"/>, applies
    /// fullscreen immediately via <see cref="Screen.fullScreen"/>, and closes the settings panel.
    /// </summary>
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

    /// <summary>
    /// Reads persisted settings from <see cref="PlayerPrefs"/> (with sensible defaults),
    /// applies them to all controls, and synchronises the value labels.
    /// </summary>
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

    #endregion
}