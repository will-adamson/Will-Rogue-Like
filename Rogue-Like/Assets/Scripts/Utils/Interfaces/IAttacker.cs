using UnityEngine;

public interface IAttacker
{
    void Attack(Vector2 direction);
    bool CanAttack { get; }
}