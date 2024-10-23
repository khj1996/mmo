using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Google.Protobuf.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_SimpleTextPopup : UI_Popup
{
    public TMP_Text Content;

    public override void Init()
    {
        base.Init();


        CloseBtn.gameObject.BindEvent(ClosePopupUI);
    }

    public void SetText(string title, string text)
    {
        Title.text = title;
        Content.text = text;
    }
}