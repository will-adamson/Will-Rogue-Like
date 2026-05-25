using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(UIDocument))]
public class CharacterSelectMenuController : MonoBehaviour
{
    [SerializeField] private string mainMenuScene = "Main Menu";
    [SerializeField] private string gameScene = "Game";

    private string selectedClass = "Warrior";

    private readonly string[] passives =
    {
        "Passive: Iron Skin — take 1 less damage from all hits",
        "Passive: Shadow Step — first attack each floor deals double damage",
        "Passive: Arcane Mind — spells cost 1 less mana"
    };

    private readonly string[] classes = { "Warrior", "Rogue", "Mage" };

    private VisualElement root;

    private void OnEnable()
    {
        root = GetComponent<UIDocument>().rootVisualElement;

        root.Q<VisualElement>("card-warrior").RegisterCallback<ClickEvent>(_ => SelectClass(0));
        root.Q<VisualElement>("card-rogue").RegisterCallback<ClickEvent>(_ => SelectClass(1));
        root.Q<VisualElement>("card-mage").RegisterCallback<ClickEvent>(_ => SelectClass(2));

        root.Q<Button>("btn-back").clicked += () => SceneManager.LoadScene(mainMenuScene);
        root.Q<Button>("btn-start").clicked += StartRun;

        SelectClass(0);
    }

    private void SelectClass(int index)
    {
        selectedClass = classes[index];

        string[] cardNames = { "card-warrior", "card-rogue", "card-mage" };
        for (int i = 0; i < cardNames.Length; i++)
        {
            VisualElement card = root.Q<VisualElement>(cardNames[i]);
            if (i == index)
                card.AddToClassList("card-selected");
            else
                card.RemoveFromClassList("card-selected");
        }

        root.Q<Label>("info-passive").text = passives[index];
    }

    private void StartRun()
    {
        PlayerPrefs.SetString("selectedClass", selectedClass);
        PlayerPrefs.SetString("hasSave", "true");
        PlayerPrefs.Save();
        SceneManager.LoadScene(gameScene);
    }
}