using System.Collections.Generic;
using UnityEngine;

public class ObjectPoolManager : MonoBehaviour
{
    private static ObjectPoolManager _instance;
    public static ObjectPoolManager Instance
    {
        get
        {
            if (_instance == null)
            {
                GameObject obj = new GameObject("Object Pool Manager");
                _instance = obj.AddComponent<ObjectPoolManager>();
                DontDestroyOnLoad(obj);
            }
            return _instance;
        }
    }

    private readonly Dictionary<GameObject, Queue<GameObject>> pools = new();

    private void Awake()
    {
        if (_instance != null && _instance != this) { Destroy(gameObject); return; }
        _instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void Prewarm(GameObject prefab, int count)
    {
        Queue<GameObject> pool = GetOrCreatePool(prefab);
        for (int i = 0; i < count; i++)
        {
            GameObject obj = Instantiate(prefab, transform);
            obj.SetActive(false);
            pool.Enqueue(obj);
        }
    }

    public GameObject Get(GameObject prefab)
    {
        Queue<GameObject> pool = GetOrCreatePool(prefab);
        GameObject obj = pool.Count > 0 ? pool.Dequeue() : Instantiate(prefab, transform);
        return obj;
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