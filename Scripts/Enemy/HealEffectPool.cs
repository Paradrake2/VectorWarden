using System.Collections.Generic;
using UnityEngine;

public static class HealEffectPool
{
    static readonly Dictionary<GameObject, Queue<GameObject>> pools = new();

    public static GameObject Get(GameObject prefab, Vector3 pos)
    {
        if (!pools.TryGetValue(prefab, out var q))
        {
            q = new Queue<GameObject>();
            pools[prefab] = q;
        }

        GameObject go = q.Count > 0 ? q.Dequeue() : Object.Instantiate(prefab);
        go.transform.position = pos;
        go.SetActive(true);
        return go;
    }

    public static void Return(GameObject prefab, GameObject instance)
    {
        instance.SetActive(false);
        pools[prefab].Enqueue(instance);
    }
}

public class TimedReturn : MonoBehaviour
{
    GameObject _prefab;
    float _t;
    bool _running;

    public static void ToPool(GameObject go, float seconds)
    {
        var tr = go.GetComponent<TimedReturn>() ?? go.AddComponent<TimedReturn>();
        tr._prefab = tr._prefab ?? go;   // store key on first use; if you spawn variants, pass the prefab explicitly
        tr._t = seconds;
        tr._running = true;
    }

    void Update()
    {
        if (!_running) return;
        _t -= Time.deltaTime;
        if (_t <= 0f)
        {
            _running = false;
            // assume the original prefab reference is stored externally:
            //HealEffectPool.Return(gameObject, gameObject); // if you have multiple prefabs, keep a mapping to the key
        }
    }
}
