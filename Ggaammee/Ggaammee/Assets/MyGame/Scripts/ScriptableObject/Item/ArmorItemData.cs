using UnityEngine;

[CreateAssetMenu(fileName = "ArmorItemData", menuName = "Inventory/ArmorItemData")]
public class ArmorItemData : EuqipItemData
{
    public int Defence = 1;

    public override Item CreateItem()
    {
        return new ArmorItem(this);
    }
}