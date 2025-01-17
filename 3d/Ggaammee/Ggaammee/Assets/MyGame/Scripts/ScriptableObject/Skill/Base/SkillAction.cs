using UnityEngine;

public class SkillAction : ScriptableObject
{
    public virtual void InvokeSkill(MonsterData.SkillData data, Transform caster, Transform target, float power)
    {
    }

    public virtual void SetPath(MonsterData.SkillData data, Transform caster, Transform target, ref Vector3[] curvePointsArr)
    {
    }
}