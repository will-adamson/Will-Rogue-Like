using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(UIDocument))]
public class SettingsController : MonoBehaviour
{
    [SerializeField] private string mainMenuScene = "Main Menu";

    private Slider sliderMaster;
    private Slider sliderMusic;
    private Slider sliderSfx;

    private Toggle toggleFullscreen;
    private Toggle toggleColorblind;
    private Toggle toggleScreenShake;

    private Label labelMaster;
    private Label labelMusic;
    private Label labelSfx;

    private void OnEnable()
    {
        VisualElement root = GetComponent<UIDocument>().rootVisualElement;

        sliderMaster = root.Q<Slider>("slider-master");
        sliderMusic = root.Q<Slider>("slider-music");
        sliderSfx = root.Q<Slider>("slider-sfx");

        toggleFullscreen = root.Q<Toggle>("toggle-fullscreen");
        toggleColorblind = root.Q<Toggle>("toggle-colorblind");
        toggleScreenShake = root.Q<Toggle>("toggle-screenshake");

        labelMaster = root.Q<Label>("label-master");
        labelMusic = root.Q<Label>("label-music");
        labelSfx = root.Q<Label>("label-sfx");

        LoadSettings();

        sliderMaster.RegisterValueChangedCallback(e => labelMaster.text = Mathf.RoundToInt(e.newValue).ToString());
        sliderMusic.RegisterValueChangedCallback(e => labelMusic.text = Mathf.RoundToInt(e.newValue).ToString());
        sliderSfx.RegisterValueChangedCallback(e => labelSfx.text = Mathf.RoundToInt(e.newValue).ToString());

        root.Q<Button>("btn-back").clicked += () => SceneManager.LoadScene(mainMenuScene);
        root.Q<Button>("btn-apply").clicked += ApplySettings;
    }

    private void LoadSettings()
    {
        sliderMaster.value = PlayerPrefs.GetFloat("vol_master", 80f);
        sliderMusic.value = PlayerPrefs.GetFloat("vol_music", 70f);
        sliderSfx.value = PlayerPrefs.GetFloat("vol_sfx", 90f);
        toggleFullscreen.value = PlayerPrefs.GetInt("fullscreen", 1) == 1;
        toggleColorblind.value = PlayerPrefs.GetInt("colorblind", 0) == 1;
        toggleScreenShake.value = PlayerPrefs.GetInt("screenshake", 1) == 1;

        labelMaster.text = Mathf.RoundToInt(sliderMaster.value).ToString();
        labelMusic.text = Mathf.RoundToInt(sliderMusic.value).ToString();
        labelSfx.text = Mathf.RoundToInt(sliderSfx.value).ToString();
    }

    private void ApplySettings()
    {
        PlayerPrefs.SetFloat("vol_master", sliderMaster.value);
        PlayerPrefs.SetFloat("vol_music", sliderMusic.value);
        PlayerPrefs.SetFloat("vol_sfx", sliderSfx.value);
        PlayerPrefs.SetInt("fullscreen", toggleFullscreen.value ? 1 : 0);
        PlayerPrefs.SetInt("colorblind", toggleColorblind.value ? 1 : 0);
        PlayerPrefs.SetInt("screenshake", toggleScreenShake.value ? 1 : 0);
        PlayerPrefs.Save();

        Screen.fullScreen = toggleFullscreen.value;

        SceneManager.LoadScene(mainMenuScene);
    }
}
