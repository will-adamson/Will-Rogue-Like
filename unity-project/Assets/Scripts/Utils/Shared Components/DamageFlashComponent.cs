using System.Collections;
using UnityEngine;

public class DamageFlashComponent : MonoBehaviour
{
    [SerializeField] private float flashDuration = 0.1f;
    [SerializeField] private Color flashColor = Color.red;

    private SpriteRenderer spriteRenderer;

    public void Init(SpriteRenderer sr) => spriteRenderer = sr;

    public void PlayHitFlash() => StartCoroutine(FlashRoutine());

    private IEnumerator FlashRoutine()
    {
        if (spriteRenderer == null) yield break;
        spriteRenderer.color = flashColor;
        yield return new WaitForSeconds(flashDuration);

        if (spriteRenderer.color == flashColor)
            spriteRenderer.color = Color.white;
    }

    public void ResetColor()
    {
        StopAllCoroutines();
        if (spriteRenderer != null)
            spriteRenderer.color = Color.white;
    }
}