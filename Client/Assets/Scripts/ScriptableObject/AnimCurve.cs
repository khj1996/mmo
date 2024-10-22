using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "SO/AnimCurve", fileName = "AnimCurve")]
public class AnimCurve : ScriptableObject
{
    [SerializeField] public AnimationCurve curve;
}