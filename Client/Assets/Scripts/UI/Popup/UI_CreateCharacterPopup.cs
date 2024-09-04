using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_CreateCharacterPopup : UI_Popup
{
    enum GameObjects
    {
        CharacterName,
    }

    enum Buttons
    {
        CreateBtn,
        CancelBtn
    }


    public override void Init()
    {
        base.Init();
        
        Bind<Button>(typeof(Buttons));
        Bind<GameObject>(typeof(GameObjects));

        GetButton((int)Buttons.CreateBtn).gameObject.BindEvent(OnClickCreate);
        GetButton((int)Buttons.CancelBtn).gameObject.BindEvent(OnClickCancel);
    }


    void OnClickCreate(PointerEventData evt)
    {
        C_CreatePlayer createPacket = new C_CreatePlayer();
        string account =
            createPacket.Name = Get<GameObject>((int)GameObjects.CharacterName).GetComponent<InputField>().text;
        Managers.Network.Send(createPacket);
    }

    void OnClickCancel(PointerEventData evt)
    {
        Managers.UI.ClosePopupUI();
    }
}