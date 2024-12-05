using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[Serializable]
public abstract class QuestData : ScriptableObject
{
    public string id;
    public string title;
    public string description;
    public abstract QuestType Type { get; }

    public List<RewardItem> rewardItems;


    [Serializable]
    public struct RewardItem
    {
        public ItemData itemData;
        public int count;
    }
}