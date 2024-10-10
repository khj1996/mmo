using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_CreateCharacterPopup : UI_Popup
{
    public TMP_InputField CharacterName;
    public Button CreateBtn;
    public Button CancelBtn;


    public override void Init()
    {
        base.Init();


        CreateBtn.gameObject.BindEvent(OnClickCreate);
        CancelBtn.gameObject.BindEvent(OnClickCancel);
    }


    void OnClickCreate(PointerEventData evt)
    {
        C_CreatePlayer createPacket = new C_CreatePlayer()
        {
            Name = CharacterName.text
        };
        Managers.Network.Send(createPacket);

        Managers.UI.ClosePopupUI();
    }

    void OnClickCancel(PointerEventData evt)
    {
        Managers.UI.ClosePopupUI();
    }
}