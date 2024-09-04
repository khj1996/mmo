using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_SelectCharacterPopup_Item : UI_Base
{
    public CharacterInfo Info { get; set; }

    enum Buttons
    {
        SelectCharacterButton
    }

    enum TMP_Texts
    {
        NameText,
        LvText,
        Map,
        PosX,
        PosY,
    }

    public override void Init()
    {
        Bind<Button>(typeof(Buttons));
        Bind<TMP_Text>(typeof(TMP_Texts));

        GetButton((int)Buttons.SelectCharacterButton).gameObject.BindEvent(OnClickButton);
    }

    public void RefreshUI()
    {
        if (Info == null)
            return;

        GetTMP((int)TMP_Texts.NameText).text = Info.PlayerName;
        GetTMP((int)TMP_Texts.LvText).text = $"레벨: {Info.Lv.ToString()}";
        GetTMP((int)TMP_Texts.Map).text = $"맵: {Info.CurMap.ToString()}";
        GetTMP((int)TMP_Texts.PosX).text = $"X: {Info.PosX:0.#}";
        GetTMP((int)TMP_Texts.PosY).text = $"Y: {Info.PosY:0.#}";
    }

    void OnClickButton(PointerEventData evt)    
    {
        if (Info.PlayerName != "캐릭터 추가")
        {
            C_EnterGame enterGamePacket = new C_EnterGame
            {
                Name = Info.PlayerName
            };
            Managers.Network.Send(enterGamePacket);
            Managers.Scene.LoadScene(Define.Scene.Game);
            Managers.UI.ClosePopupUI();
        }
        else 
        {
            Managers.UI.ShowPopupUI<UI_CreateCharacterPopup>();
        }

    }
}