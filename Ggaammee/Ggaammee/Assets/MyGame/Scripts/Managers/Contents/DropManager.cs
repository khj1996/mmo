using UnityEngine;

public class DropManager
{
    private string poolName = "DropItem";
    
    public DropManager()
    {
        Managers.PoolManager.PrewarmPools<DropItem>(poolName);
    }

    public void DropItem(ItemData itemData, Vector3 dropPosition)
    {
        var item = Managers.PoolManager.GetFromPool<DropItem>(poolName);

        Vector3 randomOffset = new Vector3(Random.Range(-1f, 1f), 0.5f, Random.Range(-1f, 1f));
        Vector3 dropForce = randomOffset.normalized * Random.Range(2f, 5f);

        item.Initialize(itemData,dropPosition + randomOffset, dropForce);
    }
}