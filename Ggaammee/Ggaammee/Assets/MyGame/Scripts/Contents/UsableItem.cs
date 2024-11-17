using System;
using UnityEngine;

public class UsableItem : StackableItem, IUsableItem
{
    public event Action UseItem;

    public UsableItem(UsableItemData data, int amount = 1) : base(data, amount)
    {
        UseItem += () => { Debug.Log("아이템 사용"); };
    }

    public void Use()
    {
        UseItem?.Invoke();
    }

    protected override StackableItem Clone(int amount)
    {
        return new UsableItem(StackableDataData as UsableItemData, amount);
    }
}