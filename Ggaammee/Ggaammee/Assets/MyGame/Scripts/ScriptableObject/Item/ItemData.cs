using UnityEngine;

public abstract class ItemData : ScriptableObject
{
    public string id;
    public string name;
    [TextArea(2, 4)] public string description;
    public Sprite itemSprite;
    public int cellPrice;
    public abstract Item CreateItem();
}