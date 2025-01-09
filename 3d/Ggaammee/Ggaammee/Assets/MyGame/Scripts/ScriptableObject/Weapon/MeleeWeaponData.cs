using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "MeleeWeaponData", menuName = "ScriptableObjects/Weapon/MeleeWeaponData")]
public class MeleeWeaponData : WeaponData
{
    public Vector3 attackPoint;
    public float checkRadius;

    public override IEnumerator AttackCoroutine(PlayerController controller, MonsterController target = null)
    {
        controller.animator.SetTrigger(AssignAnimationIDs.AnimIDAttackTrigger);

        while (controller.animator.GetCurrentAnimatorStateInfo(1).normalizedTime < 1f)
        {
            yield return null;
        }

        controller.EndAttack();
    }

    public void OnHit(PlayerController playerController, Vector3 point)
    {
        List<CharacterController> monsters = GetMonstersInRange(point, checkRadius);
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