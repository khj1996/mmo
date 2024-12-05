using Google.Protobuf.Protocol;
using System.Collections;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerSkillController : MonoBehaviour
{
    private Coroutine _coSkillCooltime;
    private ISkill equippedSkill;


    public void SetSkillData(int skillId)
    {
        if (equippedSkill != null && equippedSkill.GetSkillId() == skillId)
            return;

        equippedSkill = CreateSkill(Managers.Data.SkillDataContainer.skillDataList.FirstOrDefault(x => x.skillId == skillId));
    }

    public void UseSkill()
    {
        if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
        {
            if (equippedSkill == null || _coSkillCooltime != null)
                return;

            _coSkillCooltime = StartCoroutine(CoInputCooltime(equippedSkill.UseSkill(transform.position)));
        }
    }


    private IEnumerator CoInputCooltime(float time)
    {
        yield return new WaitForSeconds(time);
        _coSkillCooltime = null;
    }

    public ISkill CreateSkill(SkillData skillData)
    {
        switch (skillData.skillType)
        {
            case SkillType.SkillAuto:
                return new MeleeSkill(skillData);
            case SkillType.SkillProjectile:
                return new RangedSkill(skillData);
        }

        return null;
    }
}