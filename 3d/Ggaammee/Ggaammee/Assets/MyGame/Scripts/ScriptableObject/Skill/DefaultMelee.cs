using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DefaultMelee", menuName = "ScriptableObjects/Skill/DefaultMelee")]
public class DefaultMelee : SkillAction
{
    public override void InvokeSkill(MonsterData.SkillData data, Transform caster, Vector3 attackPos, float power)
    {
        List<CharacterController> players = Managers.ObjectManager.GetTargetInRange(Util.GetModifiedPoint(caster, attackPos), LayerData.PlayerLayer, data.attackEffectRadiusSqr);

        foreach (CharacterController player in players)
        {
            player.gameObject.GetComponent<PlayerController>().GetDamaged(power);
        }
    }
}