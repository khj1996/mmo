using System.Collections;
using DG.Tweening;
using UnityEngine;


[CreateAssetMenu(fileName = "RangedWeaponData", menuName = "ScriptableObjects/Weapon/RangedWeaponData")]
public class RangedWeaponData : WeaponData
{
    public ComplexCurveDesignerAsset _complexCurveDesigner;
    public float damageCoolTime = 0.4f;
    private WaitForSeconds WaitForSeconds => new WaitForSeconds(damageCoolTime);

    public override IEnumerator AttackCoroutine(PlayerController controller, MonsterController target = null)
    {
        controller.animator.SetTrigger(AssignAnimationIDs.AnimIDAttackTrigger);

        while (controller.animator.GetCurrentAnimatorStateInfo(1).normalizedTime < 1f || attackType == 2)
        {
            if (!Input.GetMouseButton(0) || target is null || !target.isActiveAndEnabled)
            {
                break;
            }

            target.GetDamaged(controller.stat.CurrentAttackPower * damageMultiply);
            EffectProcess(controller.transform, target.lockOnPos.position);

            yield return WaitForSeconds;
        }

        controller.animator.SetTrigger(AssignAnimationIDs.AnimIDEndChannelingTrigger);

        controller.EndAttack();
    }

    public void EffectProcess(Transform controllerTransform, Vector3 targetTransformPosition)
    {
        var bullet = Managers.PoolManager.GetFromPool<Bullet>("Bullet");
        if (bullet == null)
            Debug.Log(123);

        var designer = _complexCurveDesigner.GetCurveDesigner();
        var curveSet = designer.GetNextCurveSet();

        DOVirtual.Float(0f, 1f, 1, t => { bullet.transform.position = designer.EvaluateBasePosition(targetTransformPosition, controllerTransform.position, t, curveSet); })
            .OnComplete(bullet.ReturnToPool)
            .SetEase(Ease.Linear);
    }
}