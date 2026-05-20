using UnityEngine;

[CreateAssetMenu(fileName = "KnightData", menuName = "Scriptable Objects/Players/KnightData")]
public class KnightData : MeleeData
{
    [Header("Knight Stats")]
    public float blockDamageReduction = 0.5f; // TODO: For future blocking mechanic
    public float chargeSpeed = 8f; // TODO: For future charge mechanic            
}