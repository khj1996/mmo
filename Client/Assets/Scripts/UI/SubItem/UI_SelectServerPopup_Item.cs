using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_SelectServerPopup_Item : UI_Base
{
    public Button SelectServerButton;
    public TMP_Text NameText;

    public ServerInfo Info { get; set; }

    public override void Init()
    {
        SelectServerButton.gameObject.BindEvent(OnClickButton);
    }

    public void RefreshUI()
    {
        if (Info == null)
            return;

        NameText.text = Info.Name;
    }

    void OnClickButton(PointerEventData evt)
    {
        if (Managers.Network.isConnected)
        {
            Managers.Network.Connect(Info);
        }
        else
        {
            Managers.Scene.LoadScene("Login");
        }

        Managers.UI.ClosePopupUI();
    }
}