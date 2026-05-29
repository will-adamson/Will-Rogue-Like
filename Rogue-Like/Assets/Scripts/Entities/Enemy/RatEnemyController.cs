using UnityEngine;

public class RatEnemyController : MeleeEnemyController
{
    [Header("Rat Behaviour")]
    [SerializeField] private float panicSpeedMultiplier = 1.75f;

    private bool isPanicking = false;

    protected override void Awake()
    {
        base.Awake();
        Health.OnHealthChanged += OnHealthChanged;
    }

    private void OnHealthChanged(float current, float max)
    {
        if (isPanicking) return;
        if (current <= max * 0.5f)
        {
            isPanicking = true;
            MovementComp.SetSpeed(Data.speed * panicSpeedMultiplier);
        }
    }
}