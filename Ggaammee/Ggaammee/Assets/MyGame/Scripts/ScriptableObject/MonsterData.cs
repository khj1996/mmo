using System;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "NewMonster", menuName = "ScriptableObjects/Monster")]
public class MonsterData : CreatureData
{
    [Header("-------------------MonsterData--------------------")]
    public GameObject creatureModel;


    public float detectionRange;

    public List<DropItem> dropItems;
    public float exp;


    [Serializable]
    public struct DropItem
    {
        public int Id;
        public float DropRate;
    }
}