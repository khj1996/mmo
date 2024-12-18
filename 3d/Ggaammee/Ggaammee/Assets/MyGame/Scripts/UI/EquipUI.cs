﻿using System;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;

public class EquipUI : UI_Base
{
    [Serializable]
    struct EquipSlot
    {
        public EquipType type;
        public ItemSlotUI slotUI;
    }

    [SerializeField] private EquipSlot[] equipSlots;

    public TMP_Text NameText;
    public TMP_Text HpValueText;
    public TMP_Text AttackValueText;
    public TMP_Text DefenceValueText;

    private bool isInit = false;

    public override async void Init()
    {
        isInit = true;

        foreach (var slot in equipSlots)
        {
            slot.slotUI.OnDropItem -= HandleEquipSlotChange;
            slot.slotUI.OnDropItem += HandleEquipSlotChange;
        }

        Managers.InventoryManager.OnEquipChanged -= RefreshUI;
        Managers.InventoryManager.OnEquipChanged += RefreshUI;


        var playerStat = await WaitForPlayerStatInitialization();

        //playerStat.OnChangeCurrentMaxHp -= RefreshStatValue;
        //playerStat.OnChangeCurrentMaxHp += RefreshStatValue;

        playerStat.OnChangeCurrentDefensePower -= RefreshStatValue;
        playerStat.OnChangeCurrentDefensePower += RefreshStatValue;

        playerStat.OnChangeCurrentAttackPower -= RefreshStatValue;
        playerStat.OnChangeCurrentAttackPower += RefreshStatValue;

        gameObject.SetActive(false);
    }

    private async UniTask<CreatureStats> WaitForPlayerStatInitialization()
    {
        while (Managers.ObjectManager.MainPlayer?.stat == null)
        {
            await UniTask.Yield(); // 매 프레임 대기
        }

        return Managers.ObjectManager.MainPlayer.stat;
    }

    private void OnEnable()
    {
        RefreshUI();
    }

    private void HandleEquipSlotChange(ItemSlotUI fromSlot, ItemSlotUI toSlot)
    {
        var equipItem = fromSlot.Item;

        if (equipItem?.Data is EquipItemData equipData)
        {
            var targetSlot = Array.Find(equipSlots, slot => slot.slotUI == toSlot);

            if (targetSlot.slotUI && targetSlot.type == equipData.type)
            {
                Managers.InventoryManager.EquipItem(fromSlot);
            }
        }
    }

    public void RefreshStatValue(float value, StatType type)
    {
        switch (type)
        {
            case StatType.Hp:
                HpValueText.text = value.ToString();
                break;
            case StatType.Atk:
                AttackValueText.text = value.ToString();
                break;
            case StatType.Defense:
                DefenceValueText.text = value.ToString();
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(type), type, null);
        }
    }

    public void RefreshUI(EquipType type)
    {
        var currentEquipDic = Managers.InventoryManager.EquipedItems;

        var targetSlot = Array.Find(equipSlots, slot => slot.type == type);
        if (targetSlot.slotUI)
        {
            currentEquipDic.TryGetValue(type, out var equip);
            targetSlot.slotUI.SetItem(equip);
        }
    }

    public void RefreshUI()
    {
        if (!isInit)
            return;

        var currentEquipDic = Managers.InventoryManager.EquipedItems;

        foreach (var slotData in equipSlots)
        {
            currentEquipDic.TryGetValue(slotData.type, out var equip);
            slotData.slotUI.SetItem(equip);
        }

        var playerStat = Managers.ObjectManager.MainPlayer.stat;
        HpValueText.text = playerStat.CurrentMaxHp.ToString();
        AttackValueText.text = playerStat.CurrentAttackPower.ToString();
        DefenceValueText.text = playerStat.CurrentDefensePower.ToString();
    }
}