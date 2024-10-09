using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_SelectCharacterPopup_Item : UI_Base
{
    public Button SelectCharacterButton;
    public TMP_Text NameText;
    public TMP_Text LvText;
    public TMP_Text Map;
    public TMP_Text PosX;
    public TMP_Text PosY;

    public CharacterInfo Info { get; set; }


    public override void Init()
    {
        SelectCharacterButton.gameObject.BindEvent(OnClickButton);
    }

    public void RefreshUI()
    {
        if (Info == null)
            return;

        NameText.text = Info.PlayerName;
        LvText.text = $"레벨: {Info.Lv.ToString()}";
        Map.text = $"맵: {Info.CurMap.ToString()}";
        PosX.text = $"X: {Info.PosX:0.#}";
        PosY.text = $"Y: {Info.PosY:0.#}";
    }

    async void OnClickButton(PointerEventData evt)
    {
        if (Info.PlayerName != "캐릭터 추가")
        {
            C_EnterGame enterGamePacket = new C_EnterGame
            {
                Name = Info.PlayerName
            };
            Managers.Network.Send(enterGamePacket);
            Managers.Scene.LoadScene("Game");
            Managers.UI.ClosePopupUI();
        }
        else
        {
            await Managers.UI.ShowPopupUI<UI_CreateCharacterPopup>();
        }
    }
}