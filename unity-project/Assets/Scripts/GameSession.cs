using UnityEngine;

public static class GameSession
{
    public static PlayerData SelectedClass { get; private set; }

    public static void SetClass(PlayerData data)
    {
        SelectedClass = data;
    }

    public static void Clear()
    {
        SelectedClass = null;
    }
}