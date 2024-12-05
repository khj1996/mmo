using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class EquipItem : Item
{
    public EquipItemData EquipmentData { get; private set; }


    public EquipItem(EquipItemData data) : base(data)
    {
        EquipmentData = data;
    }
}