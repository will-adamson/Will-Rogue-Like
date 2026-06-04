using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

/// <summary>
/// Controls the in-game pause menu, settings overlay, and unsaved-changes prompt
/// via three layered UI Toolkit documents. Responds to the "Pause" input action
/// and manages <see cref="Time.timeScale"/> for freeze/resume.
/// </summary>
/// <remarks>
/// Document sorting order: pause at 10, settings at 11, unsaved-changes prompt at 12.
/// Settings changes set <see cref="hasUnsavedChanges"/>; if the player tries to leave
/// without applying, the unsaved-changes prompt is shown with three resolution options:
/// apply and leave, discard, or cancel.
/// The Pause input action is read from the "Player" action map of <see cref="inputActions"/>.
/// All button delegates are cleaned up in <see cref="OnDestroy"/>.
/// </remarks>
public class PauseMenuController : MonoBehaviour
{
    #region Inspector Fields

    [Header("UI Documents")]
    /// <summary>UI Document for the main pause menu panel.</summary>
    [SerializeField] private UIDocument pauseDocument;

    /// <summary>UI Document for the settings overlay.</summary>
    [SerializeField] private UIDocument settingsDocument;

    /// <summary>UI Document for the unsaved-changes confirmation prompt.</summary>
    [SerializeField] private UIDocument unsavedChangesDocument;

    [Header("Scenes")]
    /// <summary>Build name of the scene loaded when the player abandons their run.</summary>
    [SerializeField] private string mainMenuSceneName = "Main Menu";

    [Header("Input")]
    /// <summary>Input Action Asset containing the "Player" action map with a "Pause" action.</summary>
    [SerializeField] private InputActionAsset inputActions;

    #endregion

    #region Cached UI Elements — Pause Menu

    private Button btnResume;
    private Button btnInventory;
    private Button btnSettings;
    private Button btnMenu;

    #endregion

    #region Cached UI Elements — Settings

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

    #region Cached UI Elements — Unsaved Changes Prompt

    private Button btnApplyLeave;
    private Button btnDiscard;
    private Button btnCancel;

    #endregion

    #region Private State

    /// <summary>The "Pause" <see cref="InputAction"/> resolved from <see cref="inputActions"/>.</summary>
    private InputAction pauseAction;

    /// <summary>Snapshot of master volume at the last successful <see cref="ApplySettings"/> call.</summary>
    private float savedMaster;

    /// <summary>Snapshot of music volume at the last successful <see cref="ApplySettings"/> call.</summary>
    private float savedMusic;

    /// <summary>Snapshot of SFX volume at the last successful <see cref="ApplySettings"/> call.</summary>
    private float savedSfx;

    /// <summary>Snapshot of fullscreen state at the last successful <see cref="ApplySettings"/> call.</summary>
    private bool savedFullscreen;

    /// <summary>Snapshot of colourblind mode state at the last successful <see cref="ApplySettings"/> call.</summary>
    private bool savedColorblind;

    /// <summary>Snapshot of screen-shake state at the last successful <see cref="ApplySettings"/> call.</summary>
    private bool savedScreenshake;

    /// <summary>
    /// <see langword="true"/> when a settings control has been changed since the last apply or discard.
    /// Triggers the unsaved-changes prompt when the settings panel is closed without saving.
    /// </summary>
    private bool hasUnsavedChanges = false;

    #endregion

    #region Unity Lifecycle

    /// <summary>
    /// Sets up all three panels, configures sorting orders, hides overlay documents,
    /// binds the Pause input action, and loads persisted settings.
    /// </summary>
    private void Awake()
    {
        SetupPauseMenu();
        SetupSettings();
        SetupUnsavedChangesPrompt();

        pauseDocument.sortingOrder = 10;
        settingsDocument.sortingOrder = 11;
        unsavedChangesDocument.sortingOrder = 12;

        pauseDocument.rootVisualElement.style.display = DisplayStyle.None;
        settingsDocument.rootVisualElement.style.display = DisplayStyle.None;
        unsavedChangesDocument.rootVisualElement.style.display = DisplayStyle.None;

        pauseAction = inputActions.FindActionMap("Player").FindAction("Pause");
        pauseAction.performed += OnPausePressed;
        pauseAction.Enable();

        LoadSettings();
    }

    /// <summary>
    /// Unsubscribes all button and input action delegates to prevent memory leaks.
    /// </summary>
    private void OnDestroy()
    {
        btnResume.clicked -= ResumeGame;
        btnInventory.clicked -= OpenInventory;
        btnSettings.clicked -= OpenSettings;
        btnMenu.clicked -= AbandonRun;

        btnSettingsBack.clicked -= TryCloseSettings;
        btnSettingsApply.clicked -= ApplySettings;

        btnApplyLeave.clicked -= OnApplyAndLeave;
        btnDiscard.clicked -= OnDiscardChanges;
        btnCancel.clicked -= OnCancelPrompt;

        pauseAction.performed -= OnPausePressed;
        pauseAction.Disable();
    }

    #endregion

    #region Pause Menu Setup & Handlers

    /// <summary>
    /// Queries the pause document for buttons and subscribes their click handlers.
    /// </summary>
    private void SetupPauseMenu()
    {
        VisualElement root = pauseDocument.rootVisualElement;

        btnResume = root.Q<Button>("btn-resume");
        btnInventory = root.Q<Button>("btn-inventory");
        btnSettings = root.Q<Button>("btn-settings");
        btnMenu = root.Q<Button>("btn-menu");

        btnResume.clicked += ResumeGame;
        btnInventory.clicked += OpenInventory;
        btnSettings.clicked += OpenSettings;
        btnMenu.clicked += AbandonRun;
    }

    /// <summary>
    /// Toggles pause state based on which panels are currently visible.
    /// Closes the prompt first, then settings if open, then toggles the pause menu.
    /// </summary>
    /// <param name="ctx">The input action callback context (only <c>performed</c> phase is handled).</param>
    private void OnPausePressed(InputAction.CallbackContext ctx)
    {
        if (!ctx.performed) return;

        if (IsPromptOpen())
            ClosePrompt();
        else if (IsSettingsOpen())
            TryCloseSettings();
        else
            TogglePause();
    }

    /// <summary>
    /// Toggles between paused and resumed states based on the current <see cref="Time.timeScale"/>.
    /// </summary>
    private void TogglePause()
    {
        if (Time.timeScale == 0f)
            ResumeGame();
        else
            PauseGame();
    }

    /// <summary>
    /// Freezes time and shows the pause document.
    /// </summary>
    private void PauseGame()
    {
        Time.timeScale = 0f;
        pauseDocument.rootVisualElement.style.display = DisplayStyle.Flex;
    }

    /// <summary>
    /// Restores time and hides all pause-related documents.
    /// </summary>
    private void ResumeGame()
    {
        Time.timeScale = 1f;
        pauseDocument.rootVisualElement.style.display = DisplayStyle.None;
        settingsDocument.rootVisualElement.style.display = DisplayStyle.None;
        unsavedChangesDocument.rootVisualElement.style.display = DisplayStyle.None;
    }

    /// <summary>
    /// Resumes the game and opens the inventory. Not yet fully implemented.
    /// </summary>
    private void OpenInventory()
    {
        ResumeGame();
        Debug.Log("Open Inventory");
    }

    /// <summary>
    /// Restores time and loads the main menu scene, ending the current run.
    /// </summary>
    private void AbandonRun()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(mainMenuSceneName);
    }

    #endregion

    #region Settings Setup & Handlers

    /// <summary>
    /// Queries the settings document for all controls and wires up value-change callbacks
    /// that update labels and set <see cref="hasUnsavedChanges"/>.
    /// </summary>
    private void SetupSettings()
    {
        VisualElement root = settingsDocument.rootVisualElement;

        btnSettingsBack = root.Q<Button>("btn-back");
        btnSettingsApply = root.Q<Button>("btn-apply");

        btnSettingsBack.clicked += TryCloseSettings;
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

        RegisterSettingsCallbacks();
    }

    /// <summary>
    /// Registers value-changed callbacks on all settings controls.
    /// Each callback updates the adjacent label and marks <see cref="hasUnsavedChanges"/>.
    /// </summary>
    private void RegisterSettingsCallbacks()
    {
        sliderMaster.RegisterValueChangedCallback(e => { labelMaster.text = Mathf.RoundToInt(e.newValue).ToString(); hasUnsavedChanges = true; });
        sliderMusic.RegisterValueChangedCallback(e => { labelMusic.text = Mathf.RoundToInt(e.newValue).ToString(); hasUnsavedChanges = true; });
        sliderSfx.RegisterValueChangedCallback(e => { labelSfx.text = Mathf.RoundToInt(e.newValue).ToString(); hasUnsavedChanges = true; });

        toggleFullscreen.RegisterValueChangedCallback(e => hasUnsavedChanges = true);
        toggleColorblind.RegisterValueChangedCallback(e => hasUnsavedChanges = true);
        toggleScreenshake.RegisterValueChangedCallback(e => hasUnsavedChanges = true);
    }

    /// <summary>
    /// Hides the pause menu and shows the settings panel, resetting the unsaved-changes flag.
    /// </summary>
    private void OpenSettings()
    {
        hasUnsavedChanges = false;
        pauseDocument.rootVisualElement.style.display = DisplayStyle.None;
        settingsDocument.rootVisualElement.style.display = DisplayStyle.Flex;
    }

    /// <summary>
    /// Attempts to close the settings panel. If there are unsaved changes, opens the
    /// unsaved-changes prompt instead of closing immediately.
    /// </summary>
    private void TryCloseSettings()
    {
        if (hasUnsavedChanges)
            OpenPrompt();
        else
            CloseSettings();
    }

    /// <summary>
    /// Hides the settings panel and returns to the pause menu, clearing the unsaved-changes flag.
    /// </summary>
    private void CloseSettings()
    {
        hasUnsavedChanges = false;
        settingsDocument.rootVisualElement.style.display = DisplayStyle.None;
        pauseDocument.rootVisualElement.style.display = DisplayStyle.Flex;
    }

    /// <summary>
    /// Persists all current control values to <see cref="PlayerPrefs"/>, snapshots them
    /// into the <c>saved*</c> fields, applies fullscreen immediately, clears
    /// <see cref="hasUnsavedChanges"/>, and closes the settings panel.
    /// </summary>
    private void ApplySettings()
    {
        savedMaster = sliderMaster.value;
        savedMusic = sliderMusic.value;
        savedSfx = sliderSfx.value;
        savedFullscreen = toggleFullscreen.value;
        savedColorblind = toggleColorblind.value;
        savedScreenshake = toggleScreenshake.value;

        PlayerPrefs.SetFloat("vol-master", savedMaster);
        PlayerPrefs.SetFloat("vol-music", savedMusic);
        PlayerPrefs.SetFloat("vol-sfx", savedSfx);
        PlayerPrefs.SetInt("fullscreen", savedFullscreen ? 1 : 0);
        PlayerPrefs.SetInt("colorblind", savedColorblind ? 1 : 0);
        PlayerPrefs.SetInt("screenshake", savedScreenshake ? 1 : 0);
        PlayerPrefs.Save();

        Screen.fullScreen = savedFullscreen;

        hasUnsavedChanges = false;
        CloseSettings();
    }

    /// <summary>
    /// Reads persisted settings from <see cref="PlayerPrefs"/> (with sensible defaults),
    /// applies them to all controls, synchronises value labels, and clears <see cref="hasUnsavedChanges"/>.
    /// </summary>
    private void LoadSettings()
    {
        hasUnsavedChanges = false;

        savedMaster = PlayerPrefs.GetFloat("vol-master", 80f);
        savedMusic = PlayerPrefs.GetFloat("vol-music", 70f);
        savedSfx = PlayerPrefs.GetFloat("vol-sfx", 90f);
        savedFullscreen = PlayerPrefs.GetInt("fullscreen", 1) == 1;
        savedColorblind = PlayerPrefs.GetInt("colorblind", 0) == 1;
        savedScreenshake = PlayerPrefs.GetInt("screenshake", 1) == 1;

        sliderMaster.value = savedMaster;
        sliderMusic.value = savedMusic;
        sliderSfx.value = savedSfx;
        toggleFullscreen.value = savedFullscreen;
        toggleColorblind.value = savedColorblind;
        toggleScreenshake.value = savedScreenshake;

        labelMaster.text = Mathf.RoundToInt(savedMaster).ToString();
        labelMusic.text = Mathf.RoundToInt(savedMusic).ToString();
        labelSfx.text = Mathf.RoundToInt(savedSfx).ToString();

        hasUnsavedChanges = false;
    }

    #endregion

    #region Unsaved Changes Prompt Setup & Handlers

    /// <summary>
    /// Queries the unsaved-changes document for its three resolution buttons and subscribes their handlers.
    /// </summary>
    private void SetupUnsavedChangesPrompt()
    {
        VisualElement root = unsavedChangesDocument.rootVisualElement;

        btnApplyLeave = root.Q<Button>("btn-apply-leave");
        btnDiscard = root.Q<Button>("btn-discard");
        btnCancel = root.Q<Button>("btn-cancel");

        btnApplyLeave.clicked += OnApplyAndLeave;
        btnDiscard.clicked += OnDiscardChanges;
        btnCancel.clicked += OnCancelPrompt;
    }

    /// <summary>Shows the unsaved-changes prompt overlay.</summary>
    private void OpenPrompt()
    {
        unsavedChangesDocument.rootVisualElement.style.display = DisplayStyle.Flex;
    }

    /// <summary>Hides the unsaved-changes prompt overlay.</summary>
    private void ClosePrompt()
    {
        unsavedChangesDocument.rootVisualElement.style.display = DisplayStyle.None;
    }

    /// <summary>
    /// Applies pending settings and closes both the prompt and settings panels.
    /// </summary>
    private void OnApplyAndLeave()
    {
        ClosePrompt();
        ApplySettings();
    }

    /// <summary>
    /// Reverts all settings controls to their last-saved values, clears
    /// <see cref="hasUnsavedChanges"/>, and closes the prompt and settings panels.
    /// </summary>
    private void OnDiscardChanges()
    {
        hasUnsavedChanges = false;

        sliderMaster.value = savedMaster;
        sliderMusic.value = savedMusic;
        sliderSfx.value = savedSfx;
        toggleFullscreen.value = savedFullscreen;
        toggleColorblind.value = savedColorblind;
        toggleScreenshake.value = savedScreenshake;

        labelMaster.text = Mathf.RoundToInt(savedMaster).ToString();
        labelMusic.text = Mathf.RoundToInt(savedMusic).ToString();
        labelSfx.text = Mathf.RoundToInt(savedSfx).ToString();

        hasUnsavedChanges = false;
        ClosePrompt();
        CloseSettings();
    }

    /// <summary>
    /// Dismisses the unsaved-changes prompt and returns the player to the settings panel.
    /// </summary>
    private void OnCancelPrompt()
    {
        ClosePrompt();
    }

    #endregion

    #region Helpers

    /// <summary>
    /// Returns <see langword="true"/> if the settings document is currently visible.
    /// </summary>
    private bool IsSettingsOpen() =>
        settingsDocument.rootVisualElement.style.display == DisplayStyle.Flex;

    /// <summary>
    /// Returns <see langword="true"/> if the unsaved-changes prompt is currently visible.
    /// </summary>
    private bool IsPromptOpen() =>
        unsavedChangesDocument.rootVisualElement.style.display == DisplayStyle.Flex;

    #endregion
}