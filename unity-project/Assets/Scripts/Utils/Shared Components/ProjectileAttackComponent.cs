using System.Collections;
using UnityEngine;

public class ProjectileAttackComponent : MonoBehaviour, IAttacker
{
    public bool CanAttack { get; private set; } = true;

    private ProjectileData projectileData;
    private float damage;
    private float speed;
    private float range;
    private float cooldown;
    private string projectileLayer;

    public void Init(ProjectileData projectileData, float bonusDamage = 0f, string projectileLayer = "Player Projectile")
    {
        this.projectileData = projectileData;
        this.projectileLayer = projectileLayer;
        damage = projectileData.damage + bonusDamage;
        speed = projectileData.speed;
        range = projectileData.range;
        cooldown = projectileData.cooldown;

        ObjectPoolManager.Instance.Prewarm(projectileData.projectilePrefab, 10);
    }

    public void Attack(Vector2 direction)
    {
        if (!CanAttack) return;
        StartCoroutine(FireRoutine(direction));
    }

    private IEnumerator FireRoutine(Vector2 direction)
    {
        CanAttack = false;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        GameObject obj = ObjectPoolManager.Instance.Get(projectileData.projectilePrefab);
        obj.transform.SetPositionAndRotation(transform.position, Quaternion.Euler(0f, 0f, angle));
        int layer = LayerMask.NameToLayer(projectileLayer);
        obj.layer = layer;
        foreach (Transform child in obj.transform)
            child.gameObject.layer = layer;
        obj.GetComponent<Projectile>().Init(damage, speed, range, projectileData.projectilePrefab);
        obj.SetActive(true);

        yield return new WaitForSeconds(cooldown);
        CanAttack = true;
    }
}