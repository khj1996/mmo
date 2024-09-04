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
    enum Buttons
    {
        CloseBtn
    }

    enum TMP_Texts
    {
        Title,
        Content,
    }

    public override void Init()
    {
        base.Init();

        Bind<Button>(typeof(Buttons));
        Bind<TMP_Text>(typeof(TMP_Texts));

        GetButton((int)Buttons.CloseBtn).gameObject.BindEvent(OnClickButton);
    }

    public void SetText(string title, string text)
    {
        GetTMP((int)TMP_Texts.Title).text = title;
        GetTMP((int)TMP_Texts.Content).text = text;
    }

    void OnClickButton(PointerEventData evt)
    {
        Managers.UI.ClosePopupUI();
    }
}