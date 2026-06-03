using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class MoveComponent : MonoBehaviour, IMovable
{
    private Rigidbody2D rb;
    private float baseSpeed;
    private float currentSpeed;

    public void Init(Rigidbody2D rb, float speed)
    {
        this.rb = rb;
        baseSpeed = speed;
        currentSpeed = speed;

        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.linearDamping = 10f;
        rb.angularDamping = 0f;
        rb.gravityScale = 0f;
        rb.freezeRotation = true;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
    }

    public void Move(Vector2 direction)
    {
        rb.linearVelocity = direction * currentSpeed;
    }

    public void SetSpeed(float speed) => currentSpeed = speed;
    public void ResetSpeed() => currentSpeed = baseSpeed;
    public float GetBaseSpeed() => baseSpeed;
}