using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Google.Protobuf.Collections;
using Google.Protobuf.Protocol;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class UI_UseItem : UI_Popup
{
    public Button ok;
    public UI_Item _item;
    public TMP_Text description;

    public override void Init()
    {
        CloseBtn.gameObject.BindEvent(ClosePopupUI);
    }


    public void OpenPopUp(Item item)
    {
        description.text = item.Description;
        _item.SetItem(item);
        ok.gameObject.BindEvent(evt =>
        {
            item.useItem();
            Managers.UI.ClosePopupUI(this);
        });
    }
}