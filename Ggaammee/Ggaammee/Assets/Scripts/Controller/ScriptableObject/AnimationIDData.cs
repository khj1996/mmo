using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AnimationIDData", menuName = "ScriptableObjects/AnimationIDData")]
public class AnimationIDData : ScriptableObject
{
    [System.Serializable]
    public class AnimationID
    {
        public string animationName;
        [HideInInspector] public int animationHash;

        public void GenerateHash()
        {
            animationHash = Animator.StringToHash(animationName);
        }
    }

    public List<AnimationID> animationIDs = new List<AnimationID>();

    public int GetAnimationHash(string animationName)
    {
        foreach (var animID in animationIDs)
        {
            if (animID.animationName == animationName)
                return animID.animationHash;
        }

        return 0;
    }

    public void GenerateAllHashes()
    {
        foreach (var animID in animationIDs)
        {
            animID.GenerateHash();
        }
    }
}