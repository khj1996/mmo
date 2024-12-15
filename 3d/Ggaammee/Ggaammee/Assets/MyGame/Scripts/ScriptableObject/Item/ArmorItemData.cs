using UnityEngine;

[CreateAssetMenu(fileName = "ArmorItemData", menuName = "ScriptableObjects/Inventory/ArmorItemData")]
public class ArmorItemData : EquipItemData
{
    public int Defence = 1;

    public override Item CreateItem()
    {
        return new ArmorItem(this);
    }
}