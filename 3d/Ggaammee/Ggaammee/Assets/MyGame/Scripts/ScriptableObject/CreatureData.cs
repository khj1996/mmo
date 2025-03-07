using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class CreatureData : ScriptableObject
{

    [Header("Basic Info")] public string creatureId;
    public string creatureName;
    public int level;

    [Header("Stats")] public float maxHp;
    public int attack;
    public int defense;
    public float speed;
    public float sprintSpeed;
    public float crouchSpeed;
    public float acceleration;
    public float weight;

    [Header("Animations & Sounds")] public AnimationData[] animations;
    public AudioClip walkSound;

    [Serializable]
    public struct AnimationData
    {
        public string Name;
        public AnimationClip AnimationClip;
    }
}