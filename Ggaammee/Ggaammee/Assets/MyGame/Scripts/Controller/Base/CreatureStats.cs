using System;
using UnityEngine;

public class CreatureStats
{
    public float baseMaxHp;
    public int baseAttackPower;
    public int baseDefensePower;

    private float equipmentHpBonus;
    private int equipmentAttackBonus;
    private int equipmentDefenseBonus;

    public float CurrentMaxHp => baseMaxHp + equipmentHpBonus;
    public int CurrentAttackPower => baseAttackPower + equipmentAttackBonus;
    public int CurrentDefensePower => baseDefensePower + equipmentDefenseBonus;

    public float currentHp = 0;

    private bool isPlayerData;

    public CreatureStats(CreatureData creatureData)
    {
        baseMaxHp = creatureData.maxHp;
        currentHp = CurrentMaxHp;
        baseAttackPower = creatureData.attack;
        baseDefensePower = creatureData.defense;

        isPlayerData = creatureData is PlayerData;

        if (!isPlayerData) return;
        Managers.InventoryManager.OnEquipChanged -= RefreshEquipStat;
        Managers.InventoryManager.OnEquipChanged += RefreshEquipStat;
    }

    public void ResetData(CreatureData creatureData)
    {
        baseMaxHp = creatureData.maxHp;
        currentHp = CurrentMaxHp;
        baseAttackPower = creatureData.attack;
        baseDefensePower = creatureData.defense;
        ChangeHpEvent?.Invoke();
    }

    //public event Action<float, StatType> OnChangeCurrentMaxHp;
    public event Action<float, StatType> OnChangeCurrentAttackPower;
    public event Action<float, StatType> OnChangeCurrentDefensePower;

    public event Action ChangeHpEvent;

    public void RefreshEquipStat(EquipType type)
    {
        if (!isPlayerData) return;

        EquipItem equip = Managers.InventoryManager.GetEquippedItem(type);

        if (equip == null)
        {
            ResetEquipBonus(type);
            return;
        }

        if (equip.Data is ArmorItemData ai)
        {
            equipmentDefenseBonus = ai.Defence;
            OnChangeCurrentDefensePower?.Invoke(CurrentDefensePower, StatType.Defense);
        }
        else if (equip.Data is WeaponItemData wi)
        {
            equipmentAttackBonus = wi.damage;
            OnChangeCurrentAttackPower?.Invoke(CurrentAttackPower, StatType.Atk);
        }
    }

    private void ResetEquipBonus(EquipType type)
    {
        switch (type)
        {
            case EquipType.Armor:
                equipmentDefenseBonus = 0;
                OnChangeCurrentDefensePower?.Invoke(CurrentDefensePower, StatType.Defense);
                break;
            case EquipType.Weapon:
                equipmentAttackBonus = 0;
                OnChangeCurrentAttackPower?.Invoke(CurrentAttackPower, StatType.Atk);
                break;
        }
    }

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