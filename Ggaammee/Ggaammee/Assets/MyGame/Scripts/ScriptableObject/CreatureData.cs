using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class CreatureData : ScriptableObject
{
    [Header("Basic Info")] public string name;
    public int level;
   
    [Header("Stats")] public float maxHp;
    public float attack;
    public float defense;
    public float attackSpeed;
    public float speed;
    public float sprintSpeed;
    public float crouchSpeed;
    public float acceleration;
    public float weight;

    [Header("Abilities & Behavior")] public float sqrAttackRange;
    

    [Header("Animations & Sounds")] public AnimationData[] animations;
    public AudioClip walkSound;


    [Serializable]
    public struct AnimationData
    {
        public string Name;
        public AnimationClip AnimationClip;
    }


    public abstract class State<T> where T : CreatureController
    {
        protected T _owner;

        public State(T owner)
        {
            _owner = owner;
        }

        public abstract void OnEnter();
        public abstract void OnExit();
        public abstract void OnUpdate();
    }
}