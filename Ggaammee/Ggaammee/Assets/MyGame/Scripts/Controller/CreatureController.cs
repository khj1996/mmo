using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreatureController : MonoBehaviour
{
    public CreatureData creatureData;

    private float hp;

    public Animator _animator;
    protected bool _hasAnimator;
    public Transform AttackPoint;


    protected virtual void Init()
    {
        _hasAnimator = TryGetComponent(out _animator);
    }

}