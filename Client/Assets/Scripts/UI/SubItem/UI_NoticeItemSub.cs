using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_NoticeItemSub : UI_Base
{
    public CanvasGroup canvasGroup;
    public Image icon;
    public TMP_Text name;

    public override void Init()
    {
    }

    public void SetItemInfo(Item item)
    {
        icon.sprite = Managers.Data.ItemImageSO.ItemImageStructs.First(x => x.DataKey == item.TemplateId).Image;
        name.text = item.Name;
    }
}