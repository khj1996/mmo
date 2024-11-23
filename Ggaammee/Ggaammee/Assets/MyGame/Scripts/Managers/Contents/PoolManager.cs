using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Object = UnityEngine.Object;

public class PoolManager
{
    private Transform poolContainerTransform = GameObject.FindWithTag("PoolContainer").transform;
    private Dictionary<string, Queue<Poolable>> pools = new();
    private Dictionary<string, AsyncOperationHandle<GameObject>> loadedPrefabs = new();

    private const int PREFAB_POOL_SIZE = 10;

    public void PrewarmPools<T>(string name) where T : Poolable
    {
        if (!pools.ContainsKey(name))
        {
            pools[name] = new Queue<Poolable>();
        }


        var handle = Addressables.LoadAssetAsync<GameObject>(name);
        handle.Completed += (op) =>
        {
            if (op.Status == AsyncOperationStatus.Succeeded)
            {
                GameObject prefab = op.Result;

                for (int i = 0; i < PREFAB_POOL_SIZE; i++)
                {
                    T newItem = Object.Instantiate(prefab, poolContainerTransform).GetComponent<T>();
                    newItem.poolName = name;
                    newItem.OnReturnToPool();
                    pools[name].Enqueue(newItem);
                }
            }
            else
            {
                Debug.LogError($"Failed to load Addressable prefab for {name}");
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
            Debug.Log("풀 이름 설정 필요");
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