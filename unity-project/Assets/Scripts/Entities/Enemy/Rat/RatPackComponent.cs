using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MoveComponent))]
public class RatPackComponent : MonoBehaviour
{
    [Header("Pack Config")]
    [SerializeField] private float packDetectionRadius = 6f;
    [SerializeField] private string packMemberTag = "Small Rat";

    [Header("Pack Buffs")]
    [SerializeField] private float speedBuffMultiplier = 1.3f;
    [SerializeField] private float damageBuffMultiplier = 1.5f;

    private MeleeEnemyData baseData;
    private MoveComponent moveComp;
    private List<GameObject> packMembers = new List<GameObject>();
    private bool isBuffed = false;

    public bool IsBuffed => isBuffed;

    private void Awake()
    {
        moveComp = GetComponent<MoveComponent>();
    }

    public void Init(MeleeEnemyData data)
    {
        baseData = data;
        ScanForPackMembers();
    }

    private void ScanForPackMembers()
    {
        packMembers.Clear();
        Collider2D[] hits = Physics2D.OverlapCircleAll(
            transform.position, packDetectionRadius, LayerMask.GetMask("Enemy"));

        foreach (Collider2D col in hits)
        {
            if (col.gameObject == gameObject) continue;
            if (col.CompareTag(packMemberTag))
                packMembers.Add(col.gameObject);
        }

        RefreshBuffState();
    }

    public void OnPackMemberKilled(GameObject member)
    {
        packMembers.Remove(member);
        RefreshBuffState();

        HUDController.Instance.LogFeed.LogSystem($"Pack member lost. {packMembers.Count} remaining.");
    }

    private void RefreshBuffState()
    {
        bool shouldBeBuffed = packMembers.Count > 0;

        if (shouldBeBuffed == isBuffed) return;

        isBuffed = shouldBeBuffed;

        if (isBuffed)
            ApplyBuff();
        else
            RemoveBuff();
    }

    private void ApplyBuff()
    {
        moveComp.SetSpeed(baseData.speed * speedBuffMultiplier);
        HUDController.Instance.LogFeed.LogSystem($"{gameObject.name} is empowered by the pack.");
    }

    private void RemoveBuff()
    {
        moveComp.ResetSpeed();
        HUDController.Instance.LogFeed.LogSystem($"{gameObject.name} pack is broken. Power fading.");
    }

    public float GetDamageMultiplier() => isBuffed ? damageBuffMultiplier : 1f;
}