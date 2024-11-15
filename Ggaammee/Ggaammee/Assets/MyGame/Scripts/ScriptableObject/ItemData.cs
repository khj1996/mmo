using UnityEngine;

[CreateAssetMenu(fileName = "ItemData", menuName = "Inventory/ItemData")]
public class ItemData : ScriptableObject
{
    public int id;  
    public string name;    
    [TextArea(1,4)]
    public string description;    
    public Sprite itemSprite;    

}