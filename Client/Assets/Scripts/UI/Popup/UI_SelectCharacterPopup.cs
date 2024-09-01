using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

    public void SetCharacter(RepeatedField<Google.Protobuf.Protocol.LobbyPlayerInfo> playerInfos)
    {
        Items.Clear();

        GameObject grid = GetComponentInChildren<GridLayoutGroup>().gameObject;
        foreach (Transform child in grid.transform)
            Destroy(child.gameObject);

        for (int i = 0; i < playerInfos.Count; i++)
        {
            GameObject go = Managers.Resource.Instantiate("UI/Popup/UI_SelectCharacterPopup_Item", grid.transform);
            UI_SelectCharacterPopup_Item item = go.GetOrAddComponent<UI_SelectCharacterPopup_Item>();
            Items.Add(item);

            item.Info = new CharacterInfo()
            {
                PlayerName = playerInfos[i].Name,
                Lv = playerInfos[i].StatInfo.Level,
                CurMap = playerInfos[i].PosInfo.Map, 
                PosX = playerInfos[i].PosInfo.PosX,
                PosY = playerInfos[i].PosInfo.PosY,
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