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
    public Button CloseBtn;
    public TMP_Text Title;
    public TMP_Text Content;

    public override void Init()
    {
        base.Init();


        CloseBtn.gameObject.BindEvent(OnClickButton);
    }

    public void SetText(string title, string text)
    {
        Title.text = title;
        Content.text = text;
    }

    void OnClickButton(PointerEventData evt)
    {
        Managers.UI.ClosePopupUI();
    }
}