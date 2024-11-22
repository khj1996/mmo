using UnityEngine;

[CreateAssetMenu(fileName = "WeaponItemData", menuName = "Inventory/WeaponItemData")]
public class WeaponItemData : EquipItemData
{
    public int damage = 1;


    public override Item CreateItem()
    {
        return new WeaponItem(this);
    }
}