using UnityEngine;

public class SlimeStimulus : MonoBehaviour
{
    public enum StimulusType { Heat, Light, Sound }

    [SerializeField] private StimulusType type;
    [SerializeField] private float radius = 4f;
    [SerializeField] private float duration = 3f;

    private float timer;

    public float Radius => radius;
    public StimulusType Type => type;

    void Update()
    {
        if (duration <= 0f) return;
        timer += Time.deltaTime;
        if (timer >= duration) Destroy(gameObject);
    }
}