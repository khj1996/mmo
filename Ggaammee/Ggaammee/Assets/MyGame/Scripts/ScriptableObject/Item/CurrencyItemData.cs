using UnityEngine;

[CreateAssetMenu(fileName = "CurrencyItemData", menuName = "Inventory/CurrencyItemData")]
public class CurrencyItemData : StackableItemData
{
    public override Item CreateItem()
    {
        return new CurrencyItem(this);
    }
}