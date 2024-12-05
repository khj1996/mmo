using System;

public class CurrencyItem : StackableItem
{
    public CurrencyItem(CurrencyItemData data, int amount = 1) : base(data, amount)
    {
    }

    protected override StackableItem Clone(int amount)
    {
        return new CurrencyItem(StackableDataData as CurrencyItemData, amount);
    }
}