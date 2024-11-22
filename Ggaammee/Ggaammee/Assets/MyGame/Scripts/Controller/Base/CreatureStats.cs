using System;
using System.Collections.Generic;
using UnityEngine;

public class CreatureStats
{
    // 기본 스탯
    public float baseMaxHp;
    public int baseAttackPower;
    public int baseDefensePower;

    // 장비에 의한 보정값
    private float equipmentHpBonus;
    private int equipmentAttackBonus;
    private int equipmentDefenseBonus;

    // 현재 스탯 (장비와 아이템을 적용한 후 계산된 최종 스탯)
    public float CurrentMaxHp => baseMaxHp + equipmentHpBonus;
    public int CurrentAttackPower => baseAttackPower + equipmentAttackBonus;
    public int CurrentDefensePower => baseDefensePower + equipmentDefenseBonus;

    public float currentHp = 0;

    // 장비 관리
    private List<EquipItem> equippedItems = new List<EquipItem>();

    public CreatureStats(CreatureData creatureData)
    {
        baseMaxHp = creatureData.maxHp;
        currentHp = CurrentMaxHp;
        baseAttackPower = creatureData.attack;
        baseDefensePower = creatureData.defense;
    }


    public event Action<float, Util.StatType> OnChangeCurrentMaxHp;
    public event Action<float, Util.StatType> OnChangeCurrentAttackPower;
    public event Action<float, Util.StatType> OnChangeCurrentDefensePower;

    public event Action ChangeHpEvent;

    // 장비 착용
    public void Equip(EquipItem equip)
    {
        equippedItems.Add(equip);

        if (equip.Data is ArmorItemData ai)
        {
            equipmentDefenseBonus += ai.Defence;
            OnChangeCurrentDefensePower?.Invoke(CurrentDefensePower, Util.StatType.Defense);
        }
        else if (equip.Data is WeaponItemData wi)
        {
            equipmentAttackBonus += wi.damage;
            OnChangeCurrentAttackPower?.Invoke(CurrentAttackPower, Util.StatType.Atk);
        }
    }

    // 장비 해제
    public void Unequip(EquipItem equip)
    {
        equippedItems.Remove(equip);

        if (equip.Data is ArmorItemData ai)
        {
            equipmentDefenseBonus -= ai.Defence;
        }
        else if (equip.Data is WeaponItemData wi)
        {
            equipmentAttackBonus -= wi.damage;
        }
    }

    // 체력 회복
    public void HpChange(float amount)
    {
        currentHp = Mathf.Min(currentHp + amount, CurrentMaxHp);
        ChangeHpEvent?.Invoke();
    }

    // 공격력 증가
    public void IncreaseAttackPower(int amount)
    {
        baseAttackPower += amount;
    }
}