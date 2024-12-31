using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;

public class WeaponData : ScriptableObject
{
    public int attackType;
    public string weaponName;

    public float damageMultiply;
    public float attackCoolTime;

    public float sqrAttackRange;
    public AudioClip attackStartSfx;
    public AudioClip attackImpactSfx;

    public virtual IEnumerator AttackCoroutine(PlayerController controller, MonsterController target = null)
    {
        yield return null;
    }

    public void Initialize(Animator animator)
    {
        animator.SetInteger(AssignAnimationIDs.AnimIDAttackType, attackType);
        animator.SetFloat(AssignAnimationIDs.AnimIDAttackTypeTemp, attackType);
    }
}