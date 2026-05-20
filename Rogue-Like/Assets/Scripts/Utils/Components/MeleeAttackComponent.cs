using UnityEngine;

public class MeleeAttackComponent : MonoBehaviour
{
    private float damage;
    private float knockbackForce;
    private float attackCooldown;
    private float lastAttackTime;
    private LayerMask targetLayer;

    public void Init(float damage, float knockbackForce, float attackCooldown, string targetLayer)
    {
        this.damage = damage;
        this.knockbackForce = knockbackForce;
        this.attackCooldown = attackCooldown;
        this.targetLayer = LayerMask.GetMask(targetLayer);
    }

    public bool Attack(Vector2 dir)
    {
        if (Time.time - lastAttackTime < attackCooldown) return false;
        lastAttackTime = Time.time;

        Collider2D hit = Physics2D.OverlapCircle(
            (Vector2)transform.position + dir * 0.5f,
            0.4f,
            targetLayer
        );

        if (hit == null) return false;

        if (hit.TryGetComponent(out IDamageable entity))
            entity.TakeDamage(damage);

        if (hit.TryGetComponent(out Rigidbody2D rb))
            rb.AddForce(dir * knockbackForce, ForceMode2D.Impulse);

        return true;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, 0.4f);
    }
}