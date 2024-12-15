using UnityEngine;

[CreateAssetMenu(fileName = "WeaponItemData", menuName = "ScriptableObjects/Inventory/WeaponItemData")]
public class WeaponItemData : EquipItemData
{
    public int damage = 1;


    public override Item CreateItem()
    {
        return new WeaponItem(this);
    }
}