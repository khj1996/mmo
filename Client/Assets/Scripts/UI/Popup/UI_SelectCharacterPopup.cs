using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Google.Protobuf.Collections;
using UnityEngine;
using UnityEngine.UI;

public class UI_SelectCharacterPopup : UI_Popup
{
    public List<UI_SelectCharacterPopup_Item> Items { get; } = new List<UI_SelectCharacterPopup_Item>();

    public override void Init()
    {
        base.Init();
    }

    public async void SetCharacter(RepeatedField<Google.Protobuf.Protocol.LobbyPlayerInfo> playerInfos)
    {
        Items.Clear();

        GameObject grid = GetComponentInChildren<GridLayoutGroup>().gameObject;
        foreach (Transform child in grid.transform)
            Destroy(child.gameObject);

        var newTask = new List<UniTask<UI_SelectCharacterPopup_Item>>();

        for (int i = 0; i < playerInfos.Count; i++)
        {
            newTask.Add(Managers.UI.MakeSubItem<UI_SelectCharacterPopup_Item>(grid.transform));
        }

        Items.AddRange(await UniTask.WhenAll(newTask));


        for (int i = 0; i < playerInfos.Count; i++)
        {
            Items[i].Info = new CharacterInfo()
            {
                PlayerName = playerInfos[i].Name,
                Lv = playerInfos[i].StatInfo.Level,
                CurMap = playerInfos[i].PosInfo.Map,
                PosX = playerInfos[i].PosInfo.PosX,
                PosY = playerInfos[i].PosInfo.PosY,
            };
        }

        if (playerInfos.Count < 8)
        {
            UI_SelectCharacterPopup_Item item = await Managers.UI.MakeSubItem<UI_SelectCharacterPopup_Item>(grid.transform);
            Items.Add(item);

            item.Info = new CharacterInfo()
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

    public async void AddCharacter(Google.Protobuf.Protocol.LobbyPlayerInfo playerInfos)
    {
        Items.Last().Info = new CharacterInfo()
        {
            PlayerName = playerInfos.Name,
            Lv = playerInfos.StatInfo.Level,
            CurMap = playerInfos.PosInfo.Map,
            PosX = playerInfos.PosInfo.PosX,
            PosY = playerInfos.PosInfo.PosY,
        };

        if (Items.Count < 8)
        {
            GameObject grid = GetComponentInChildren<GridLayoutGroup>().gameObject;
            UI_SelectCharacterPopup_Item item = await Managers.UI.MakeSubItem<UI_SelectCharacterPopup_Item>(grid.transform);
            Items.Add(item);

            item.Info = new CharacterInfo()
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