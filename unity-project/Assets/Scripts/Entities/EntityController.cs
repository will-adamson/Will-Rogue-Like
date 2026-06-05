using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(SpriteRenderer), typeof(HealthComponent))]
[RequireComponent(typeof(Animator))]
public abstract class EntityController : MonoBehaviour, IDamageable
{
    public Rigidbody2D Rb { get; private set; }
    public SpriteRenderer Sprite { get; private set; }

    protected HealthComponent Health { get; private set; }
    protected Animator Anim { get; private set; }

    public bool IsDead => Health.IsDead;

    protected abstract float MaxHealth { get; }
    protected abstract float GetDefence();

    protected static readonly int HashDie = Animator.StringToHash("Die");
    protected static readonly int HashDamage = Animator.StringToHash("Damage");
    protected static readonly int HashAttack = Animator.StringToHash("Attack");

    protected virtual string DeathStateName => "Die";

    protected virtual void Awake()
    {
        Rb = GetComponent<Rigidbody2D>();
        Sprite = GetComponent<SpriteRenderer>();

        Health = GetComponent<HealthComponent>();
        Health.Init(MaxHealth);
        Health.OnDeath += HandleDeath;

        Anim = GetComponent<Animator>();
    }

    public void TakeDamage(float rawAmount)
    {
        Health.ApplyDamage(rawAmount, GetDefence());
        OnDamageTaken(rawAmount);
    }

    protected virtual void OnDamageTaken(float rawAmount)
    {
        if (IsDead) return;
        Anim.SetTrigger(HashDamage);
    }

    protected virtual void HandleDeath()
    {
        StartCoroutine(DeathRoutine());
    }

    private IEnumerator DeathRoutine()
    {
        Anim.ResetTrigger(HashDamage);
        Anim.ResetTrigger(HashAttack);
        Anim.SetTrigger(HashDie);

        yield return null;

        yield return new WaitUntil(() =>
            Anim.GetCurrentAnimatorStateInfo(0).IsName(DeathStateName) &&
            Anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f);

        OnDeathAnimationComplete();
    }

    protected virtual void OnDeathAnimationComplete()
    {
        Destroy(gameObject);
    }
}