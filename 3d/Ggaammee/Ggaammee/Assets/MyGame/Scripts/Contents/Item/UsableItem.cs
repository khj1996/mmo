using System;
using UnityEngine;

public class UsableItem : StackableItem, IUsableItem
{
    public event Action UseItem;

    public UsableItem(UsableItemData data, int amount = 1) : base(data, amount)
    {
        UseItem += () =>
        {
            SetAmount(--Count);
            Managers.ObjectManager.MainPlayer.stat.HpChange(data.value);
            EventManager.TriggerItemCollected(StackableData.id, -1);
        };
    }

    public void Use()
    {
        UseItem?.Invoke();
    }

    protected override StackableItem Clone(int amount)
    {
        return new UsableItem(StackableData as UsableItemData, amount);
    }
}