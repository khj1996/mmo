using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MeleeWeaponData", menuName = "ScriptableObjects/Weapon/MeleeWeaponData")]
public class MeleeWeaponData : WeaponData
{
    public Vector3 attackPostion = new Vector3();
    public float CheckRadius;

    public override IEnumerator AttackCoroutine(PlayerController controller)
    {
        controller.animator.SetTrigger(AssignAnimationIDs.AnimIDAttackTrigger);
        controller.animator.SetInteger(AssignAnimationIDs.AnimIDAttackType, 1);

        while (controller.animator.GetCurrentAnimatorStateInfo(1).normalizedTime < 1f)
        {
            yield return null;
        }

        controller.animator.SetInteger(AssignAnimationIDs.AnimIDAttackType, 0);
        controller.EndAttack();
    }

    public void OnHit(PlayerController playerController, Vector3 attackPoint)
    {
        List<CharacterController> monsters = GetMonstersInRange(attackPoint);
        foreach (CharacterController monster in monsters)
        {
            monster.gameObject.GetComponent<CreatureController>().GetDamaged(playerController.stat.CurrentAttackPower);
        }
    }

    public List<CharacterController> GetMonstersInRange(Vector3 position, float radius = 0.5f)
    {
        List<CharacterController> monstersInRange = new List<CharacterController>();
        Collider[] colliders = Physics.OverlapSphere(position, radius, LayerData.MonsterLayer);

        foreach (Collider collider in colliders)
        {
            CharacterController monster = collider.GetComponent<CharacterController>();
            if (monster != null)
            {
                monstersInRange.Add(monster);
            }
        }

        return monstersInRange;
    }
}