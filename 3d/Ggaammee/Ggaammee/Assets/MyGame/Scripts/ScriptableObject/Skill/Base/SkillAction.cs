using UnityEngine;

public class SkillAction : ScriptableObject
{
    public virtual void InvokeSkill(MonsterData.SkillData data, Transform caster, Vector3 attackPos, float power)
    {
    }
}