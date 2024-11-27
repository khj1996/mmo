using System;
using UnityEngine;

[Serializable]
public abstract class QuestData : ScriptableObject
{
    public string id;         
    public string title;      
    public string description;
    public int rewardGold;    
    public int rewardExp;     

    public abstract QuestType Type { get; } 

   
}