﻿using UnityEngine;

[CreateAssetMenu(fileName = "UsableItemData", menuName = "ScriptableObjects/Inventory/UsableItemData")]
public class UsableItemData : StackableItemData
{
    public float value = 1;

    public override Item CreateItem()
    {
        return new UsableItem(this);
    }
}