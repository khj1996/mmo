using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;

public class UI_Inventory : UI_Base
{
    public List<UI_Item> Items { get; } = new List<UI_Item>();

    [SerializeField] private TMP_Text _goldTMP;


    public override void Init()
    {
        Items.Clear();

        var grid = transform.Find("ItemGrid").gameObject;
        foreach (Transform child in grid.transform)
            Destroy(child.gameObject);

        for (int i = 0; i < 20; i++)
        {
            var sc = Managers.UI.MakeSubItem<UI_Item>(grid.transform);
            Items.Add(sc);
        }

        RefreshUI(Define.InvenRefreshType.All);
    }

    public void RefreshUI(Define.InvenRefreshType type)
    {
        if (type == Define.InvenRefreshType.Currency || type == Define.InvenRefreshType.All)
        {
            _goldTMP.text = Managers.Inven.Items.Values.First(x => x.TemplateId == 400000).Count.ToString();
        }


        if (Items.Count == 0)
            return;

        var items = Managers.Inven.Items.Values.Where(x => x.Slot >= 0).ToList();
        items.Sort((left, right) => left.Slot - right.Slot);

        foreach (Item item in items)
        {
            if (item.Slot < 0 || item.Slot >= 20)
                continue;

            Items[item.Slot].SetItem(item);
        }
    }
}