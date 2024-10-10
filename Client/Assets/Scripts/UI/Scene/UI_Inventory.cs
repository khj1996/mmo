using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_Inventory : UI_ScrollView<UI_Inventory_Sub>
{
    [Header("---------UI_Inventory-----------")] [SerializeField]
    private TMP_Text _goldTMP;

    private int maxLine;
    private int poolSize;
    private float prefabH;
    private int currentIndex;
    private int lastUpdateLine;


    public override void Init()
    {
        maxIndex = Managers.Inven.SlotLen / 5 + 1;


        base.Init();

        InitializeView();
    }


    public void RefreshUI(Define.InvenRefreshType type)
    {
        if (type == Define.InvenRefreshType.Currency || type == Define.InvenRefreshType.All)
        {
            var gold = Managers.Inven.Items.Values.FirstOrDefault(x => x.TemplateId == 400000)?.Count.ToString();

            _goldTMP.text = (gold == null) ? "0" : gold.ToString();
        }


        if (Managers.Inven.Items.Count == 0)
            return;

        foreach (var sub in items)
        {
            sub.RefreshUI(sub._index);
        }
    }
}