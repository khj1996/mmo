using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

public class PoolManager
{
    private Transform poolContainerTransform;
    private Dictionary<Type, Queue<Poolable>> pools;

    private void Awake()
    {
        pools = new Dictionary<Type, Queue<Poolable>>();
        poolContainerTransform = GameObject.FindWithTag("PoolContainer").transform;
    }

    public T GetFromPool<T>(T prefab) where T : Poolable
    {
        Type type = typeof(T);

        if (!pools.ContainsKey(type))
        {
            pools[type] = new Queue<Poolable>();
        }

        if (pools[type].Count > 0)
        {
            Poolable item = pools[type].Dequeue();
            item.gameObject.SetActive(true);
            item.OnGetFromPool();
            return (T)item;
        }

        T newItem = Object.Instantiate(prefab, poolContainerTransform);
        return newItem;
    }

    public void ReturnToPool(Poolable item)
    {
        Type type = item.GetType();

        if (!pools.ContainsKey(type))
        {
            pools[type] = new Queue<Poolable>();
        }

        item.gameObject.SetActive(false);
        item.OnReturnToPool();
        pools[type].Enqueue(item);
    }
}