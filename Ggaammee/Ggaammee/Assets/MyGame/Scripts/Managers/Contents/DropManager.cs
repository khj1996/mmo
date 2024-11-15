using UnityEngine;

public class DropManager
{
    [SerializeField] private DropItem dropItemPrefab;

    public void DropItem(Vector3 dropPosition)
    {
        // 풀에서 아이템 가져오기
        DropItem item = Managers.PoolManager.GetFromPool(dropItemPrefab);

        // 드랍 위치와 힘 설정
        Vector3 randomOffset = new Vector3(Random.Range(-1f, 1f), 0.5f, Random.Range(-1f, 1f));
        Vector3 dropForce = randomOffset.normalized * Random.Range(2f, 5f);

        // 아이템 초기화
        item.Initialize(dropPosition + randomOffset, dropForce);
    }
}