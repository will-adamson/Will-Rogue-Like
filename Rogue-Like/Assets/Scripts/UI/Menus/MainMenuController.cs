using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(UIDocument))]
public class MainMenuController : MonoBehaviour
{
    [SerializeField] private string newRunScene = "Character Select";
    [SerializeField] private string continueScene = "Dungeon Game";
    [SerializeField] private string settingsScene = "Settings Menu";
    [SerializeField] private string saveKey = "hasSave";

    private void OnEnable()
    {
        VisualElement root = GetComponent<UIDocument>().rootVisualElement;

        root.Q<Button>("btn-new-run").clicked += () => SceneManager.LoadScene(newRunScene);
        root.Q<Button>("btn-continue").clicked += () => SceneManager.LoadScene(continueScene);
        root.Q<Button>("btn-settings").clicked += () => SceneManager.LoadScene(settingsScene);
        root.Q<Button>("btn-quit").clicked += Quit;

        Button continueBtn = root.Q<Button>("btn-continue");
        continueBtn.SetEnabled(PlayerPrefs.HasKey(saveKey));
        continueBtn.style.opacity = PlayerPrefs.HasKey(saveKey) ? 1f : 0.35f;
    }

    private static void Quit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}