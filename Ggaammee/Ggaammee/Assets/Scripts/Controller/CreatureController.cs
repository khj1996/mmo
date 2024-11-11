using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreatureController : MonoBehaviour
{
    public CreatureData creatureData;

    protected Animator _animator;
    protected bool _hasAnimator;


    protected virtual void Init()
    {
        _hasAnimator = TryGetComponent(out _animator);
        AssignAnimationIDs();
    }

    private void AssignAnimationIDs()
    {
    }

    private void OnFootstep(AnimationEvent animationEvent)
    {
        if (animationEvent.animatorClipInfo.weight > 0.5f)
        {
            AudioSource.PlayClipAtPoint(creatureData.walkSound, transform.position, 0.5f);
        }
    }
    
    private void OnHit(AnimationEvent animationEvent)
    {
        /*if (animationEvent.animatorClipInfo.weight > 0.5f)
        {
            AudioSource.PlayClipAtPoint(creatureData.walkSound, transform.position, 0.5f);
        }*/
    }
}