using System;
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

    // CreatureData 타입이 PlayerData인지 여부를 체크
    private bool isPlayerData;

    public CreatureStats(CreatureData creatureData)
    {
        baseMaxHp = creatureData.maxHp;
        currentHp = CurrentMaxHp;
        baseAttackPower = creatureData.attack;
        baseDefensePower = creatureData.defense;

        // PlayerData인지 체크
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

    // 장비 장착 시 스탯 갱신
    public void RefreshEquipStat(EquipType type)
    {
        // PlayerData일 때만 이벤트 실행
        if (!isPlayerData) return;

        // 인벤토리에서 해당 타입의 장비를 가져옴
        EquipItem equip = Managers.InventoryManager.GetEquippedItem(type);

        // 장비가 없는 경우, 해당 타입의 보너스를 초기화
        if (equip == null)
        {
            ResetEquipBonus(type);
            return;
        }

        // 장비가 있는 경우, 타입에 따라 보너스를 갱신
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

    // 해당 타입의 장비 보너스를 초기화
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