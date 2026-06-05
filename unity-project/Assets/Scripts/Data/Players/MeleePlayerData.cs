using UnityEngine;

public abstract class MeleePlayerData : PlayerData
{
    public float attackCooldown = 0.5f;
    public float knockbackForce = 3f;
    public float attackRange = 0.6f;
}
