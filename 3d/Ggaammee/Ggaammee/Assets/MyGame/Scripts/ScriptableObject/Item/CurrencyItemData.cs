using UnityEngine;

[CreateAssetMenu(fileName = "CurrencyItemData", menuName = "ScriptableObjects/Inventory/CurrencyItemData")]
public class CurrencyItemData : StackableItemData
{
    public override Item CreateItem()
    {
        return new CurrencyItem(this);
    }
}