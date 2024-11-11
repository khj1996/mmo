using System;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "NewCreature", menuName = "ScriptableObjects/Creature")]
public class CreatureData : ScriptableObject
{
    [Header("Basic Info")] public string name;
    public int level;
    public GameObject creatureModel;

    [Header("Stats")] public float maxHp;
    public float attack;
    public float defense;
    public float atackSpeed;
    public float speed;
    public float sprintSpeed;
    public float crouchSpeed;
    public float acceleration;
    public float weight;

    [Header("Abilities & Behavior")] public float attackRange;
    public float detectionRange;

    [Header("Animations & Sounds")] public AnimationData[] animations;
    public AudioClip walkSound;

    [Header("Drops & Rewards")] public List<DropItem> dropItems;
    public float exp;


    [Serializable]
    public struct AnimationData
    {
        public string Name;
        public AnimationClip AnimationClip;
    }

    [Serializable]
    public struct DropItem
    {
        public int Id;
        public float DropRate;
    }
}