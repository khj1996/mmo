using UnityEngine;

public enum ItemType
{
    None = 0,
    Weapon = 1,
    Armor = 2,
    Usable = 3,
    Currency = 4,
}

public class ItemData : ScriptableObject
{
    public int id;
    public string name;
    [TextArea(2, 4)] public string description;
    public Sprite itemSprite;
    public ItemType itemType;
}

[CreateAssetMenu(fileName = "ItemData", menuName = "Inventory/WeaponItemData")]
public class WeaponItemData : ItemData
{
}

[CreateAssetMenu(fileName = "ItemData", menuName = "Inventory/ArmorItemData")]
public class ArmorItemData : ItemData
{
}

[CreateAssetMenu(fileName = "ItemData", menuName = "Inventory/UsableItemData")]
public class UsableItemData : ItemData
{
    public bool Stackable;
    public int maxStack;
}