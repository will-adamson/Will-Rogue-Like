using System.Collections;
using UnityEngine;
[RequireComponent(typeof(Rigidbody2D), typeof(SpriteRenderer))]
public abstract class EntityController : MonoBehaviour, IDamageable
{
    public Rigidbody2D Rb { get; private set; }
    public SpriteRenderer Sprite { get; private set; }

    public float CurrentHealth { get; private set; }
    protected abstract float MaxHealth { get; }
    public bool IsDead { get; private set; }

    [Header("Damage")]
    [SerializeField] private float flashDuration = 0.1f;
    [SerializeField] private Color flashColor = Color.red;

    protected virtual void Awake()
    {
        Rb = GetComponent<Rigidbody2D>();
        Sprite = GetComponent<SpriteRenderer>();

        CurrentHealth = MaxHealth;
    }

    public void TakeDamage(float rawAmount)
    {
        if (IsDead) return;

        float mitigated = Mathf.Max(0f, rawAmount - GetDefence());
        CurrentHealth = Mathf.Clamp(CurrentHealth - mitigated, 0f, MaxHealth);

        StartCoroutine(FlashDamageRoutine());

        if (CurrentHealth <= 0f) HandleDeath();
    }

    protected abstract float GetDefence();

    protected virtual void HandleDeath()
    {
        IsDead = true;
        Sprite.color = Color.gray;
    }

    private IEnumerator FlashDamageRoutine()
    {
        Sprite.color = flashColor;
        yield return new WaitForSeconds(flashDuration);
        if (!IsDead) Sprite.color = Color.white;
    }
}
