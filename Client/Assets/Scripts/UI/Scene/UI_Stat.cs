using Data;
using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Image = UnityEngine.UI.Image;

public class UI_Stat : UI_Base
{
    public Image Slot_Helmet;
    public Image Slot_Armor;
    public Image Slot_Boots;
    public Image Slot_Weapon;
    public Image Slot_Shield;

    public TMP_Text NameText;
    public TMP_Text AttackValueText;
    public TMP_Text DefenceValueText;


    bool _init = false;

    public override void Init()
    {
        _init = true;
        RefreshUI();
    }


    public void RefreshUI()
    {
        if (_init == false)
            return;

        Slot_Helmet.enabled = false;
        Slot_Armor.enabled = false;
        Slot_Boots.enabled = false;
        Slot_Weapon.enabled = false;
        Slot_Shield.enabled = false;

        foreach (Item item in Managers.Inven.Items.Values)
        {
            if (item.Equipped == false)
                continue;

            Managers.Data.ItemDict.TryGetValue(item.TemplateId, out ItemData itemData);

            Sprite icon = Managers.Data.ItemImageSO.ItemImageStructs.First(x => x.DataKey == itemData.id).Image;

            if (item.ItemType == ItemType.Weapon)
            {
                Slot_Weapon.enabled = true;
                Slot_Weapon.sprite = icon;
            }
            else if (item.ItemType == ItemType.Armor)
            {
                Armor armor = (Armor)item;
                switch (armor.ArmorType)
                {
                    case ArmorType.Helmet:
                        Slot_Helmet.enabled = true;
                        Slot_Helmet.sprite = icon;
                        break;
                    case ArmorType.Armor:
                        Slot_Armor.enabled = true;
                        Slot_Armor.sprite = icon;
                        break;
                    case ArmorType.Boots:
                        Slot_Boots.enabled = true;
                        Slot_Boots.sprite = icon;
                        break;
                }
            }
        }

        // Text
        MyPlayerController player = Managers.Object.MyPlayer;
        player.RefreshAdditionalStat();

        NameText.text = player.name;

        int totalDamage = player.Stat.Attack + player.WeaponDamage;
        AttackValueText.text = $"{totalDamage}(+{player.WeaponDamage})";
        DefenceValueText.text = $"{player.ArmorDefence}";
    }
}