using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Projectile : MonoBehaviour
{
    private float damage;
    private float speed;
    private float lifetime;
    private GameObject prefab;
    private Rigidbody2D rb;
    private bool isInitialised;

    public void Init(float damage, float speed, float range, GameObject prefab)
    {
        this.damage = damage;
        this.speed = speed;
        this.prefab = prefab;
        lifetime = range / speed;
        isInitialised = true;
    }

    private void OnEnable()
    {
        if (!isInitialised) return;

        rb = GetComponent<Rigidbody2D>();
        rb.linearVelocity = transform.right * speed;
        Invoke(nameof(ReturnToPool), lifetime);
    }

    private void OnDisable()
    {
        CancelInvoke();
        if (rb != null) rb.linearVelocity = Vector2.zero;
        isInitialised = false;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        collision.gameObject.GetComponent<IDamageable>()?.TakeDamage(damage);
        ReturnToPool();
    }

    private void ReturnToPool()
    {
        ObjectPoolManager.Instance.Return(prefab, gameObject);
    }
}