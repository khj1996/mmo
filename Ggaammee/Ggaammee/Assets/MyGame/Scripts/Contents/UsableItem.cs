using System;

public class UsableItem : StackableItem, IUsableItem
{
    public event Action UseItem;

    public UsableItem(UsableItemData data, int amount = 1) : base(data, amount)
    {
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