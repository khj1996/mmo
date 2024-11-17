using UnityEngine;

public abstract class ItemData : ScriptableObject
{
    public int id;
    public string name;
    [TextArea(2, 4)] public string description;
    public Sprite itemSprite;
    public abstract Item CreateItem();
}