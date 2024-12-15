using System.Collections;
using UnityEngine;


[CreateAssetMenu(fileName = "RangedWeaponData", menuName = "ScriptableObjects/Weapon/RangedWeaponData")]
public class RangedWeaponData : WeaponData
{
    //public float MaxDistance;
    public override IEnumerator AttackCoroutine(PlayerController controller)
    {
        controller.animator.SetTrigger(AssignAnimationIDs.AnimIDAttackTrigger);

        while (controller.animator.GetCurrentAnimatorStateInfo(1).normalizedTime < 1f)
        {
            yield return null;
        }

        controller.EndAttack();
    }
}