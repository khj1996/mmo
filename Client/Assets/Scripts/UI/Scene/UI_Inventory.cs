using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class UI_Inventory : UI_Base
{
    public List<UI_Item> Items { get; } = new List<UI_Item>();

    public override async void Init()
    {
        Items.Clear();

        GameObject grid = transform.Find("ItemGrid").gameObject;
        foreach (Transform child in grid.transform)
            Destroy(child.gameObject);


        var newTask = new List<UniTask<UI_Item>>();

        for (int i = 0; i < 20; i++)
        {
            newTask.Add(Managers.UI.MakeSubItem<UI_Item>(grid.transform));
        }

        Items.AddRange(await UniTask.WhenAll(newTask));

        RefreshUI();
    }

    public void RefreshUI()
    {
        if (Items.Count == 0)
            return;

        List<Item> items = Managers.Inven.Items.Values.ToList();
        items.Sort((left, right) => { return left.Slot - right.Slot; });

        foreach (Item item in items)
        {
            if (item.Slot < 0 || item.Slot >= 20)
                continue;

            Items[item.Slot].SetItem(item);
        }
    }
}