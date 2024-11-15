using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Object = UnityEngine.Object;

public class PoolManager
{
    private Transform poolContainerTransform = GameObject.FindWithTag("PoolContainer").transform;
    private Dictionary<Type, Queue<Poolable>> pools = new();
    private Dictionary<Type, AsyncOperationHandle<GameObject>> loadedPrefabs = new();

    private const int PREFAB_POOL_SIZE = 10;

    public void PrewarmPools<T>() where T : Poolable
    {
        var type = typeof(T);

        if (!pools.ContainsKey(type))
        {
            pools[type] = new Queue<Poolable>();
        }


        var handle = Addressables.LoadAssetAsync<GameObject>(type.Name);
        handle.Completed += (op) =>
        {
            if (op.Status == AsyncOperationStatus.Succeeded)
            {
                GameObject prefab = op.Result;

                for (int i = 0; i < PREFAB_POOL_SIZE; i++)
                {
                    T newItem = Object.Instantiate(prefab, poolContainerTransform).GetComponent<T>();
                    newItem.OnReturnToPool();
                    pools[type].Enqueue(newItem);
                }
            }
            else
            {
                Debug.LogError($"Failed to load Addressable prefab for {type.Name}");
            }
        };
    }


    public T GetFromPool<T>() where T : Poolable
    {
        var type = typeof(T);

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

        return null;
    }

    public void ReturnToPool(Poolable item)
    {
        Type type = item.GetType();

        if (!pools.ContainsKey(type))
        {
            pools[type] = new Queue<Poolable>();
        }

        item.OnReturnToPool();
        pools[type].Enqueue(item);
    }
}