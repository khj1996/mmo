using System.Linq;
using TMPro;
using UnityEngine;

public class GoldUI : UI_Base
{
    [SerializeField] public TMP_Text amount;

    public override void Init()
    {
        Managers.InventoryManager.CurrencyChanged -= RefreshUI;
        Managers.InventoryManager.CurrencyChanged += RefreshUI;

        RefreshUI();
    }

    private void RefreshUI()
    {
        var gold = Managers.InventoryManager.Currency.FirstOrDefault(x => x.Data.id == "item_400");

        if (gold == null)
        {
            amount.text = "0";
        }
        else
        {
            var text = Util.FormatNumber(gold.Count);

            amount.text = text;
        }
    }
}