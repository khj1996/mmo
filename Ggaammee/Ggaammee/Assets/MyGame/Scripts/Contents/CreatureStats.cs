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
    private List<EquipmentItem> equippedItems = new List<EquipmentItem>();

    // 아이템 사용에 의한 일시적 효과
    private int temporaryHpBonus;
    private int temporaryAttackBonus;

    public event Action ChangeHpEvent;

    // 아이템 효과 적용
    public void ApplyItemEffect(int value)
    {
        HpChange(value);
    }

    // 장비 착용
    public void Equip(EquipmentItem equipment)
    {
        equippedItems.Add(equipment);

        if (equipment.Data is ArmorItemData ai)
        {
            equipmentDefenseBonus += ai.Defence;
        }
        else if (equipment.Data is WeaponItemData wi)
        {
            equipmentAttackBonus += wi.damage;
        }
    }

    // 장비 해제
    public void Unequip(EquipmentItem equipment)
    {
        equippedItems.Remove(equipment);

        if (equipment.Data is ArmorItemData ai)
        {
            equipmentDefenseBonus -= ai.Defence;
        }
        else if (equipment.Data is WeaponItemData wi)
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