using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;

public class WeaponData : ScriptableObject
{
    public string weaponId;
    public string weaponName;

    public float damageMultiply;
    public float attackCoolTime;

    public float sqrAttackRange;
    public AudioClip attackStartSfx;
    public AudioClip attackImpactSfx;

    public virtual IEnumerator AttackCoroutine(PlayerController controller)
    {
        yield return null;
    }
}