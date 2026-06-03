using UnityEngine;

[CreateAssetMenu(fileName = "KnightData", menuName = "Scriptable Objects/Players/KnightData")]
public class KnightData : MeleeData
{
    /// <summary>Damage reduction when blocking. Range: 0–1. TODO: implement blocking.</summary>
    public float blockDamageReduction = 0.5f;

    /// <summary>TODO: implement charge attack.</summary>
    public float chargeSpeed = 8f;
}
