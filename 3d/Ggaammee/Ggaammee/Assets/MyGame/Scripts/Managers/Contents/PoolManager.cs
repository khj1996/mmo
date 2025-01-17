﻿using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class PoolManager
{
    private readonly Transform poolContainerTransform = GameObject.FindWithTag("PoolContainer").transform;
    private Dictionary<string, Queue<Poolable>> pools = new();

    private const int PrefabPoolSize = 10;

    public void PrewarmPools<T>(string name, Transform parent = null, int count = PrefabPoolSize) where T : Poolable
    {
        if (!pools.ContainsKey(name))
        {
            pools[name] = new Queue<Poolable>();
        }

        if (!parent)
        {
            parent = poolContainerTransform;
        }

        var handle = Addressables.LoadAssetAsync<GameObject>(name);
        handle.Completed += (op) =>
        {
            if (op.Status == AsyncOperationStatus.Succeeded)
            {
                GameObject prefab = op.Result;

                for (int i = 0; i < count; i++)
                {
                    T newItem = Object.Instantiate(prefab, parent).GetComponent<T>();
                    newItem.poolName = name;
                    newItem.OnReturnToPool();
                    pools[name].Enqueue(newItem);
                }
            }
        };
    }

    public T GetFromPool<T>(string name) where T : Poolable
    {
        if (!pools.ContainsKey(name))
        {
            pools[name] = new Queue<Poolable>();
        }

        if (pools[name].Count > 0)
        {
            Poolable item = pools[name].Dequeue();
            item.gameObject.SetActive(true);
            item.OnGetFromPool();

            return (T)item;
        }

        return null;
    }

    public void ReturnToPool(string poolName, Poolable item)
    {
        if (poolName == null)
        {
            return;
        }

        if (!pools.ContainsKey(poolName))
        {
            pools[poolName] = new Queue<Poolable>();
        }

        item.OnReturnToPool();
        pools[poolName].Enqueue(item);
    }
}