using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Google.Protobuf.Collections;
using Google.Protobuf.Protocol;
using UnityEngine;
using UnityEngine.UI;

public class UI_SelectCharacterPopup : UI_Popup
{
    public List<UI_SelectCharacterPopup_Item> Items { get; } = new List<UI_SelectCharacterPopup_Item>();

    public override void Init()
    {
        gameObject.name = "UI_SelectCharacterPopup";
        base.Init();
    }

    public void SetCharacter(RepeatedField<Google.Protobuf.Protocol.LobbyPlayerInfo> playerInfos)
    {
        Items.Clear();

        GameObject grid = GetComponentInChildren<GridLayoutGroup>().gameObject;
        foreach (Transform child in grid.transform)
            Destroy(child.gameObject);


        for (int i = 0; i < playerInfos.Count; i++)
        {
            var sc = Managers.UI.MakeSubItem<UI_SelectCharacterPopup_Item>(grid.transform);
            sc.gameObject.transform.localScale = Vector3.one;
            sc.Info = new CharacterInfo()
            {
                PlayerName = playerInfos[i].Name,
                Lv = playerInfos[i].StatInfo.Level,
                PosX = playerInfos[i].PosInfo.Pos.X,
                PosY = playerInfos[i].PosInfo.Pos.Y,
            };

            Items.Add(sc);
        }

        if (playerInfos.Count < 8)
        {
            var sc = Managers.UI.MakeSubItem<UI_SelectCharacterPopup_Item>(grid.transform);
            sc.gameObject.transform.localScale = Vector3.one;
            Items.Add(sc);

            sc.Info = new CharacterInfo()
            {
                PlayerName = "캐릭터 추가",
                Lv = 1,
                CurMap = 1,
                PosX = 0,
                PosY = 0,
            };
        }

        RefreshUI();
    }

    public void AddCharacter(LobbyPlayerInfo playerInfos)
    {
        Items.Last().Info = new CharacterInfo()
        {
            PlayerName = playerInfos.Name,
            Lv = playerInfos.StatInfo.Level,
            PosX = playerInfos.PosInfo.Pos.X,
            PosY = playerInfos.PosInfo.Pos.Y,
        };

        if (Items.Count < 8)
        {
            GameObject grid = GetComponentInChildren<GridLayoutGroup>().gameObject;
            UI_SelectCharacterPopup_Item sc = Managers.UI.MakeSubItem<UI_SelectCharacterPopup_Item>(grid.transform);
            sc.gameObject.transform.localScale = Vector3.one;
            Items.Add(sc);

            sc.Info = new CharacterInfo()
            {
                PlayerName = "캐릭터 추가",
                Lv = 1,
                CurMap = 1,
                PosX = 0,
                PosY = 0,
            };
        }

        RefreshUI();
    }

    public void RefreshUI()
    {
        if (Items.Count == 0)
            return;

        foreach (var item in Items)
        {
            item.RefreshUI();
        }
    }
}