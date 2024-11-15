using UnityEngine;

public abstract class Poolable : MonoBehaviour
{
    public abstract void OnGetFromPool();
    public abstract void OnReturnToPool();

    public void ReturnToPool()
    {
        Managers.PoolManager.ReturnToPool(this);
    }
}