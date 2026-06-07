using UnityEngine;

[CreateAssetMenu(fileName = "DwarvenFighterData", menuName = "Scriptable Objects/Players/Dwarven Fighter Data")]
public class DwarvenFighterData : MeleePlayerData
{
    /// <summary>Damage reduction when blocking. Range: 0–1. TODO: implement blocking.</summary>
    public float blockDamageReduction = 0.5f;

    /// <summary>TODO: implement charge attack.</summary>
    public float chargeSpeed = 8f;
}
