using UnityEngine;

public class MeleeAttackComponent : MonoBehaviour
{
    private float damage;
    private float knockbackForce;
    private float attackCooldown;
    private int layerMask;
    private string targetName;
    private bool isPlayerAttack;

    private float lastAttackTime = -Mathf.Infinity;

    private AttackShape attackShape;
    private float attackAngle;
    private int attackHitCount;
    private StatusEffect onHitEffect;

    public void Init(MeleeEnemyData data, string targetLayerName, string displayName)
    {
        damage = data.damage;
        knockbackForce = data.knockbackForce;
        attackCooldown = data.attackCooldown;
        layerMask = LayerMask.GetMask(targetLayerName);
        targetName = displayName;
        isPlayerAttack = false;

        attackShape = data.attackShape;
        attackAngle = data.attackAngle;
        attackHitCount = data.attackHitCount;
        onHitEffect = data.onHitEffect;
    }

    public void Init(float damage, float knockbackForce, float attackCooldown, string targetLayerName, string displayName = "Target")
    {
        this.damage = damage;
        this.knockbackForce = knockbackForce;
        this.attackCooldown = attackCooldown;
        layerMask = LayerMask.GetMask(targetLayerName);
        targetName = displayName;
        isPlayerAttack = true;

        attackShape = AttackShape.Point;
        attackAngle = 45f;
        attackHitCount = 1;
        onHitEffect = null;
    }

    public bool CanAttack() => Time.time >= lastAttackTime + attackCooldown;

    public bool Attack(Vector2 dir)
    {
        if (!CanAttack()) return false;

        lastAttackTime = Time.time;

        switch (attackShape)
        {
            case AttackShape.Point:
                ExecutePointAttack(dir);
                break;
            case AttackShape.Arc:
                ExecuteArcAttack(dir);
                break;
            case AttackShape.Radial:
                ExecuteRadialAttack();
                break;
        }

        return true;
    }

    private void ExecutePointAttack(Vector2 dir)
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, dir, 1.5f, layerMask);
        if (hit.collider != null && hit.collider.gameObject != gameObject)
            ApplyHit(hit.collider, dir);
    }

    private void ExecuteArcAttack(Vector2 dir)
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, 1.5f, layerMask);
        foreach (Collider2D col in hits)
        {
            if (col.gameObject == gameObject) continue;
            Vector2 toTarget = ((Vector2)col.transform.position - (Vector2)transform.position).normalized;
            if (Vector2.Angle(dir, toTarget) <= attackAngle * 0.5f)
                ApplyHit(col, dir);
        }
    }

    private void ExecuteRadialAttack()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, 1.5f, layerMask);
        int count = 0;
        foreach (Collider2D col in hits)
        {
            if (col.gameObject == gameObject) continue;
            if (count >= attackHitCount) break;
            Vector2 dir = ((Vector2)col.transform.position - (Vector2)transform.position).normalized;
            ApplyHit(col, dir);
            count++;
        }
    }

    private void ApplyHit(Collider2D col, Vector2 dir)
    {
        if (col.TryGetComponent(out IDamageable damageable))
        {
            damageable.TakeDamage(damage);
            if (isPlayerAttack)
            {
                string targetDisplayName = col.gameObject.name.Replace("(Clone)", "").Trim();
                targetDisplayName = string.IsNullOrEmpty(targetDisplayName) ? targetName : targetDisplayName;
                HUDController.Instance?.LogFeed.LogDamage($"You hit {targetDisplayName} for {Mathf.RoundToInt(damage)}.");
            }
        }

        if (col.TryGetComponent(out Rigidbody2D targetRb))
            targetRb.AddForce(dir * knockbackForce, ForceMode2D.Impulse);

        if (onHitEffect != null && col.TryGetComponent(out IStatusEffectable effectable))
        {
            effectable.ApplyEffect(onHitEffect);
            if (isPlayerAttack)
                HUDController.Instance?.LogFeed.LogSystem($"{onHitEffect.effectName} applied to {targetName}.");
        }
    }
}