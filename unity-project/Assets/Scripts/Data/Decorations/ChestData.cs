using UnityEngine;

[CreateAssetMenu(fileName = "ChestData", menuName = "Scriptable Objects/Decorations/Chest Data")]
public class ChestData : ScriptableObject
{
    [Header("Reward Weights")]
    public float weightEmpty      = 20f;
    public float weightCoins      = 40f;
    public float weightExperience = 30f;
    public float weightLoreNote   = 10f;

    [Header("Coin Reward")]
    public int coinsMin = 5;
    public int coinsMax = 30;

    [Header("Experience Reward")]
    public int experienceMin = 10;
    public int experienceMax = 50;

    [Header("Lore Notes")]
    public string[] loreNotes;
}