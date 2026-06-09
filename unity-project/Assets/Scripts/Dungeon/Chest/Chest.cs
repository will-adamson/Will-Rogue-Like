using UnityEngine;

public class Chest : MonoBehaviour
{
    public enum ChestRewardType { Empty, Coins, Experience, LoreNote }

    [SerializeField] private ChestData chestData;
    [SerializeField] private Animator animator;

    private bool opened = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (opened) return;
        if (!other.CompareTag("Player")) return;
        Open();
    }

    private void Open()
    {
        opened = true;

        animator?.SetTrigger("Open");

        ChestRewardType reward = RollReward();
        GrantReward(reward);
    }

    private ChestRewardType RollReward()
    {
        float total = chestData.weightEmpty
                    + chestData.weightCoins
                    + chestData.weightExperience
                    + chestData.weightLoreNote;

        float roll = Random.Range(0f, total);

        if (roll < chestData.weightEmpty)
            return ChestRewardType.Empty;

        roll -= chestData.weightEmpty;
        if (roll < chestData.weightCoins)
            return ChestRewardType.Coins;

        roll -= chestData.weightCoins;
        if (roll < chestData.weightExperience)
            return ChestRewardType.Experience;

        return ChestRewardType.LoreNote;
    }

    private void GrantReward(ChestRewardType reward)
    {
        switch (reward)
        {
            case ChestRewardType.Empty:
                Debug.Log("Chest was empty.");
                break;

            case ChestRewardType.Coins:
                int coins = Random.Range(chestData.coinsMin, chestData.coinsMax + 1);
                Debug.Log($"Chest contained {coins} coins.");
                // TODO: Add coins
                break;

            case ChestRewardType.Experience:
                int xp = Random.Range(chestData.experienceMin, chestData.experienceMax + 1);
                Debug.Log($"Chest contained {xp} experience.");
                // TODO: Add experience
                break;

            case ChestRewardType.LoreNote:
                GrantLoreNote();
                break;
        }
    }

    private void GrantLoreNote()
    {
        if (chestData.loreNotes == null || chestData.loreNotes.Length == 0)
        {
            Debug.Log("Chest contained a lore note but none are defined in ChestData.");
            return;
        }

        string note = chestData.loreNotes[Random.Range(0, chestData.loreNotes.Length)];
        Debug.Log($"Lore note found: \"{note}\"");
        // TODO: LoreManager.Instance.AddNote(note);
    }
}