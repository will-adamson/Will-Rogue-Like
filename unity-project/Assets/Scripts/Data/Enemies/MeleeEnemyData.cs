using UnityEngine;

[CreateAssetMenu(fileName = "MeleeEnemyData", menuName = "Scriptable Objects/Enemies/MeleeEnemyData")]
public class MeleeEnemyData : EnemyData
{
    [Header("Melee Stats")]
    public float damage = 10f;
    public float knockbackForce = 2f;
    public float attackCooldown = 1f;

    [Header("Attack Behaviour")]
    public AttackShape attackShape = AttackShape.Point;
    public float attackAngle = 45f; // Arc shape only
    public int attackHitCount = 1; // Radial shape only

    [Header("Movement Behaviour")]
    public MovementPattern movementPattern = MovementPattern.Direct;
    public float circleStrafeDist = 2f;
    public float chargeSpeed = 8f;
    public float chargeCooldown = 3f;
    public float zigzagAmplitude = 1.5f;
    public float zigzagFrequency = 3f;

    [Header("On-Hit Reactions")]
    public bool canBeStaggered = true;
    public float staggerThreshold = 0f;

    [Header("Special")]
    public StatusEffect onHitEffect;
    public float splitOnDeathChance = 0f;
}