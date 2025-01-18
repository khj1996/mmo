using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CircleArea", menuName = "ScriptableObjects/Skill/CircleArea")]
public class CircleArea : SkillAction
{
    public override void InvokeSkill(MonsterData.SkillData data, Transform caster, Transform target, float power)
    {
        if (Physics.Raycast(caster.position, Vector3.down, out var hit, 10f, LayerData.GroundLayer))
        {
            var test = Managers.PoolManager.GetFromPool<CircleRange>("CircleRange");

            test.StartRange(hit.point, data.attackEffectRadius, data.motionDelay - 3, () =>
            {
                List<CharacterController> players = Managers.ObjectManager.GetTargetInRange(hit.point, LayerData.PlayerLayer, data.attackEffectRadius);

                foreach (CharacterController player in players)
                {
                    player.gameObject.GetComponent<PlayerController>().GetDamaged(power);
                }
            });
        }
    }

    public override void SetPath(MonsterData.SkillData data, Transform caster, Transform target, ref Vector3[] curvePointsArr)
    {
        // attackPos의 위치만큼 이동
        // 이동 완료 후에 공격 시작
        // 데칼을 이용하여 범위 표시
        // 공격 완료 0.5초 전에 하강 시작
        // 다시 원래 상태로 복귀

        var targetPoint = target.position;
        targetPoint += data.attackPos;

        Vector3 midPoint = new Vector3((caster.position.x + targetPoint.x) / 2f, (caster.position.y + targetPoint.y) / 2f, (caster.position.z + targetPoint.z) / 2f);

        CalculateCurvePoints(ref curvePointsArr, caster.position, targetPoint, midPoint);
    }


    //경로 계산
    public static void CalculateCurvePoints(ref Vector3[] curvePointsArr, Vector3 start, Vector3 target, Vector3 curvePos, int reCalcIndex = 0)
    {
        int _count = curvePointsArr.Length;
        float unit = 1.0f / _count;
        float t = unit * reCalcIndex;
        for (int i = reCalcIndex; i < _count; i++, t += unit)
        {
            float u = (1 - t);
            float t2 = t * t;
            float u2 = u * u;

            curvePointsArr[i] =
                start * u2 +
                curvePos * (t * u * 2) +
                target * t2;
        }
    }
}