using System;
using TMPro;
using UnityEngine;

public class EquipUI : UI_Base
{
    [Serializable]
    struct EquipSlot
    {
        public Util.EquipType type;
        public ItemSlotUI slotUI;
    }

    [SerializeField] private EquipSlot[] equipSlots;

    public TMP_Text NameText;
    public TMP_Text HpValueText;
    public TMP_Text AttackValueText;
    public TMP_Text DefenceValueText;

    private bool isInit = false;

    public override void Init()
    {
        isInit = true;

        // EquipSlot에 드롭 이벤트 연결
        foreach (var slot in equipSlots)
        {
            slot.slotUI.OnDropItem += HandleEquipSlotChange;
        }

        // 인벤토리 장비 변경 이벤트 등록
        Managers.InventoryManager.OnEquipChanged += RefreshUI;

        gameObject.SetActive(false);
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
                // 장비 변경
                Managers.InventoryManager.EquipItem(fromSlot);
            }
        }
    }

    public void RefreshStatValue(float value, Util.StatType type)
    {
        switch (type)
        {
            case Util.StatType.Hp:
                HpValueText.text = value.ToString();
                break;
            case Util.StatType.Atk:
                AttackValueText.text = value.ToString();
                break;
            case Util.StatType.Defense:
                DefenceValueText.text = value.ToString();
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(type), type, null);
        }
    }

    public void RefreshUI(Util.EquipType type)
    {
        var currentEquipDic = Managers.InventoryManager.EquipedItems;

        // 해당 타입의 슬롯만 갱신
        var targetSlot = Array.Find(equipSlots, slot => slot.type == type);
        if (targetSlot.slotUI)
        {
            currentEquipDic.TryGetValue(type, out var equip);
            targetSlot.slotUI.SetItem(equip);
        }

        // 스탯 갱신
        var playerStat = Managers.ObjectManager.MainPlayer.stat;
        HpValueText.text = playerStat.CurrentMaxHp.ToString();
        AttackValueText.text = playerStat.CurrentAttackPower.ToString();
        DefenceValueText.text = playerStat.CurrentDefensePower.ToString();
    }

    public void RefreshUI()
    {
        if (!isInit)
            return;

        var currentEquipDic = Managers.InventoryManager.EquipedItems;

        // 모든 슬롯 갱신
        foreach (var slotData in equipSlots)
        {
            currentEquipDic.TryGetValue(slotData.type, out var equip);
            slotData.slotUI.SetItem(equip);
        }

        // 스탯 갱신
        var playerStat = Managers.ObjectManager.MainPlayer.stat;
        HpValueText.text = playerStat.CurrentMaxHp.ToString();
        AttackValueText.text = playerStat.CurrentAttackPower.ToString();
        DefenceValueText.text = playerStat.CurrentDefensePower.ToString();
    }
}
