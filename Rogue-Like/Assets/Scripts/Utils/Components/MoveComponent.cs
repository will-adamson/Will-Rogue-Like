using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class MoveComponent : MonoBehaviour, IMovable
{
    private Rigidbody2D rb;
    private float speed;

    public void Init(Rigidbody2D rb, float speed)
    {
        this.rb = rb;
        this.speed = speed;
    }

    public void Move(Vector2 direction)
    {
        rb.MovePosition(rb.position + speed * Time.fixedDeltaTime * direction);
    }

    public void SetSpeed(float speed) => this.speed = speed;
}