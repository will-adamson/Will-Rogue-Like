using UnityEngine;

public class MeleeAttackComponent : MonoBehaviour
{
    private float damage;
    private float knockbackForce;
    private float attackCooldown;
    private float lastAttackTime;

    public void Init(float damage, float knockbackForce, float attackCooldown)
    {
        this.damage = damage;
        this.knockbackForce = knockbackForce;
        this.attackCooldown = attackCooldown;
    }

    public void Attack(Vector2 dir)
    {
        if (Time.time - lastAttackTime < attackCooldown) return;
        lastAttackTime = Time.time;

        Collider2D hit = Physics2D.OverlapCircle(
            (Vector2)transform.position + dir * 0.5f,
            0.4f,
            LayerMask.GetMask("Player")
        );

        if (hit == null) return;

        if (hit.TryGetComponent(out EntityController entity))
            entity.TakeDamage(damage);

        if (hit.TryGetComponent(out Rigidbody2D rb))
            rb.AddForce(dir * knockbackForce, ForceMode2D.Impulse);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, 0.4f);
    }
}