using System.Collections.Generic;
using UnityEngine;

public class ObjectPoolManager : MonoBehaviour
{
    public static ObjectPoolManager Instance { get; private set; }

    private readonly Dictionary<GameObject, Queue<GameObject>> pools = new();

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    public GameObject Get(GameObject prefab)
    {
        Queue<GameObject> pool = GetOrCreatePool(prefab);

        if (pool.Count > 0)
        {
            GameObject obj = pool.Dequeue();
            obj.SetActive(true);
            return obj;
        }

        return Instantiate(prefab, transform);
    }

    public void Return(GameObject prefab, GameObject obj)
    {
        obj.SetActive(false);
        GetOrCreatePool(prefab).Enqueue(obj);
    }

    private Queue<GameObject> GetOrCreatePool(GameObject prefab)
    {
        if (!pools.ContainsKey(prefab))
            pools[prefab] = new Queue<GameObject>();

        return pools[prefab];
    }
}