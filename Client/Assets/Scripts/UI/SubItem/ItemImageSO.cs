using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "SO/ItemImage", fileName = "ItemImageStorage")]
public class ItemImageSO : ScriptableObject
{
    [SerializeField] public List<ItemImageData> ItemImageStructs;
}

[System.Serializable]
public struct ItemImageData
{
    public Sprite Image;
    public int DataKey;
}