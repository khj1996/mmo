using UnityEngine;

public abstract class Poolable : MonoBehaviour
{
    public string poolName = "";
    
    public abstract void OnGetFromPool();
    public abstract void OnReturnToPool();

    public void ReturnToPool()
    {
        Managers.PoolManager.ReturnToPool(poolName,this);
    }
}