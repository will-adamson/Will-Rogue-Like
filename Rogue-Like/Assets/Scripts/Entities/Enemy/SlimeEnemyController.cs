using System.Collections;
using UnityEngine;

public class SlimeEnemyController : MeleeEnemyController
{
    [Header("Slime Split")]
    [SerializeField] private GameObject smallSlimePrefab;
    [SerializeField] private int splitCount = 2;
    [SerializeField] private float splitSpread = 1.2f;

    private bool isSplit = false;

    protected override void HandleDeath()
    {
        MeleeEnemyData data = Data as MeleeEnemyData;

        if (!isSplit && data != null && Random.value <= data.splitOnDeathChance)
            StartCoroutine(SpawnSplitsNextFrame());

        base.HandleDeath();
    }

    private IEnumerator SpawnSplitsNextFrame()
    {
        yield return null;

        for (int i = 0; i < splitCount; i++)
        {
            float angle = (360f / splitCount) * i * Mathf.Deg2Rad;
            Vector2 offset = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * splitSpread;

            GameObject split = Instantiate(smallSlimePrefab, (Vector2)transform.position + offset, Quaternion.identity);

            if (split.TryGetComponent(out DamageFlashComponent flash))
                flash.ResetColor();

            if (split.TryGetComponent(out SlimeEnemyController splitController))
                splitController.isSplit = true;
        }
    }
}