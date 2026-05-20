using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(HealthComponent))]
[RequireComponent(typeof(Animator))]
public abstract class EntityController : MonoBehaviour, IDamageable
{
    public Rigidbody2D Rb { get; private set; }
    public SpriteRenderer Sprite { get; private set; }

    protected HealthComponent Health { get; private set; }
    protected Animator Anim { get; private set; }

    private DamageFlashComponent damageFlash;

    public bool IsDead => Health.IsDead;

    protected abstract float MaxHealth { get; }
    protected abstract float GetDefence();

    protected virtual void Awake()
    {
        Rb = GetComponent<Rigidbody2D>();
        Sprite = GetComponent<SpriteRenderer>();

        Health = GetComponent<HealthComponent>();
        Health.Init(MaxHealth);
        Health.OnDeath += HandleDeath;

        Anim = GetComponent<Animator>();

        damageFlash = GetComponent<DamageFlashComponent>();
        damageFlash?.Init(Sprite);
        if (damageFlash != null)
        {
            Health.OnHealthChanged += (_, _) => damageFlash.PlayHitFlash();
            Health.OnDeath += damageFlash.PlayDeathVisual;
        }
    }

    public void TakeDamage(float rawAmount)
    {
        Health.ApplyDamage(rawAmount, GetDefence());
    }

    protected abstract void HandleDeath();
}