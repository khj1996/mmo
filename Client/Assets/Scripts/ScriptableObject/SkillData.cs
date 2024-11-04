using System.Collections.Generic;
using Google.Protobuf.Protocol;
using UnityEngine;
using UnityEngine.Serialization;



[CreateAssetMenu(menuName = "SO/Skill/SkillData", fileName = "SkillDataStorage")]
public class SkillData : ScriptableObject
{
    public int skillId;
    public string skillName;
    public float coolTime;
    public SkillType skillType;
}

public interface ISkill
{
    public int GetSkillId();
    float UseSkill(Vector2 playerPos);
}

public class MeleeSkill : ISkill
{
    private SkillData skillData;

    public MeleeSkill(SkillData data)
    {
        skillData = data;
    }

    public int GetSkillId()
    {
        return skillData.skillId;
    }

    public float UseSkill(Vector2 playerPos)
    {
        SendSkillPacket(skillData.skillId);

        return skillData.coolTime;
    }

    private void SendSkillPacket(int skillId)
    {
        C_Skill skill = new C_Skill
        {
            Info = new SkillInfo { SkillId = skillId }
        };

        Managers.Network.Send(skill);
    }
}

public class RangedSkill : ISkill
{
    private SkillData skillData;

    public RangedSkill(SkillData data)
    {
        skillData = data;
    }

    public int GetSkillId()
    {
        return skillData.skillId;
    }

    public float UseSkill(Vector2 playerPos)
    {
        var mPos = Camera.main.ScreenPointToRay(Input.mousePosition);
        var moveDir = (new Vector2(mPos.origin.x, mPos.origin.y) - playerPos).normalized;

        SendSkillPacket(skillData.skillId, moveDir);

        return skillData.coolTime;
    }

    private void SendSkillPacket(int skillId, Vector2 direction)
    {
        C_Skill skill = new C_Skill
        {
            Info = new SkillInfo { SkillId = skillId },
            MoveDir = new Vec2 { X = direction.x, Y = direction.y }
        };
        Managers.Network.Send(skill);
    }
}