using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class PauseMenuController : MonoBehaviour
{
    [SerializeField] private UIDocument pauseDocument;
    [SerializeField] private UIDocument settingsDocument;
    [SerializeField] private UIDocument unsavedChangesDocument;
    [SerializeField] private string mainMenuSceneName = "Main Menu";
    [SerializeField] private InputActionAsset inputActions;

    private Button btnResume;
    private Button btnInventory;
    private Button btnSettings;
    private Button btnMenu;

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

    private Button btnApplyLeave;
    private Button btnDiscard;
    private Button btnCancel;

    private InputAction pauseAction;

    private float savedMaster;
    private float savedMusic;
    private float savedSfx;
    private bool savedFullscreen;
    private bool savedColorblind;
    private bool savedScreenshake;

    private bool hasUnsavedChanges = false;

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

    private void SetupPauseMenu()
    {
        var root = pauseDocument.rootVisualElement;

        btnResume = root.Q<Button>("btn-resume");
        btnInventory = root.Q<Button>("btn-inventory");
        btnSettings = root.Q<Button>("btn-settings");
        btnMenu = root.Q<Button>("btn-menu");

        btnResume.clicked += ResumeGame;
        btnInventory.clicked += OpenInventory;
        btnSettings.clicked += OpenSettings;
        btnMenu.clicked += AbandonRun;
    }

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

    private void TogglePause()
    {
        if (Time.timeScale == 0f)
            ResumeGame();
        else
            PauseGame();
    }

    private void PauseGame()
    {
        Time.timeScale = 0f;
        pauseDocument.rootVisualElement.style.display = DisplayStyle.Flex;
    }

    private void ResumeGame()
    {
        Time.timeScale = 1f;
        pauseDocument.rootVisualElement.style.display = DisplayStyle.None;
        settingsDocument.rootVisualElement.style.display = DisplayStyle.None;
        unsavedChangesDocument.rootVisualElement.style.display = DisplayStyle.None;
    }

    private void OpenInventory()
    {
        ResumeGame();
        Debug.Log("Open Inventory");
    }

    private void AbandonRun()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(mainMenuSceneName);
    }

    private void SetupSettings()
    {
        var root = settingsDocument.rootVisualElement;

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

    private void RegisterSettingsCallbacks()
    {
        sliderMaster.RegisterValueChangedCallback(e =>
        {
            labelMaster.text = Mathf.RoundToInt(e.newValue).ToString();
            hasUnsavedChanges = true;
        });

        sliderMusic.RegisterValueChangedCallback(e =>
        {
            labelMusic.text = Mathf.RoundToInt(e.newValue).ToString();
            hasUnsavedChanges = true;
        });

        sliderSfx.RegisterValueChangedCallback(e =>
        {
            labelSfx.text = Mathf.RoundToInt(e.newValue).ToString();
            hasUnsavedChanges = true;
        });

        toggleFullscreen.RegisterValueChangedCallback(e => hasUnsavedChanges = true);
        toggleColorblind.RegisterValueChangedCallback(e => hasUnsavedChanges = true);
        toggleScreenshake.RegisterValueChangedCallback(e => hasUnsavedChanges = true);
    }

    private void OpenSettings()
    {
        hasUnsavedChanges = false;
        pauseDocument.rootVisualElement.style.display = DisplayStyle.None;
        settingsDocument.rootVisualElement.style.display = DisplayStyle.Flex;
    }

    private void TryCloseSettings()
    {
        if (hasUnsavedChanges)
            OpenPrompt();
        else
            CloseSettings();
    }

    private void CloseSettings()
    {
        hasUnsavedChanges = false;
        settingsDocument.rootVisualElement.style.display = DisplayStyle.None;
        pauseDocument.rootVisualElement.style.display = DisplayStyle.Flex;
    }

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
    }

    private void SetupUnsavedChangesPrompt()
    {
        var root = unsavedChangesDocument.rootVisualElement;

        btnApplyLeave = root.Q<Button>("btn-apply-leave");
        btnDiscard = root.Q<Button>("btn-discard");
        btnCancel = root.Q<Button>("btn-cancel");

        btnApplyLeave.clicked += OnApplyAndLeave;
        btnDiscard.clicked += OnDiscardChanges;
        btnCancel.clicked += OnCancelPrompt;
    }

    private void OpenPrompt()
    {
        unsavedChangesDocument.rootVisualElement.style.display = DisplayStyle.Flex;
    }

    private void ClosePrompt()
    {
        unsavedChangesDocument.rootVisualElement.style.display = DisplayStyle.None;
    }

    private void OnApplyAndLeave()
    {
        ClosePrompt();
        ApplySettings();
    }

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

        ClosePrompt();
        CloseSettings();
    }

    private void OnCancelPrompt()
    {
        ClosePrompt();
    }

    private bool IsSettingsOpen() =>
        settingsDocument.rootVisualElement.style.display == DisplayStyle.Flex;

    private bool IsPromptOpen() =>
        unsavedChangesDocument.rootVisualElement.style.display == DisplayStyle.Flex;
}