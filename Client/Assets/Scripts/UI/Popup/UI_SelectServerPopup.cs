using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Google.Protobuf.Collections;
using UnityEngine;
using UnityEngine.UI;

public class UI_SelectServerPopup : UI_Popup
{
	public List<UI_SelectServerPopup_Item> Items { get; } = new List<UI_SelectServerPopup_Item>();

	public override void Init()
	{
		base.Init();
	}

	public void SetServers(RepeatedField<Google.Protobuf.Protocol.ServerInfo> servers)
	{
		Items.Clear();

		GameObject grid = GetComponentInChildren<GridLayoutGroup>().gameObject;
		foreach (Transform child in grid.transform)
			Destroy(child.gameObject);

		for (int i = 0; i < servers.Count; i++)
		{
			UI_SelectServerPopup_Item item = Managers.UI.MakeSubItem<UI_SelectServerPopup_Item>(grid.transform);
			Items.Add(item);

			item.Info = new ServerInfo()
			{
				Name = servers[i].Name, 
				Port = servers[i].Port, 
				BusyScore = servers[i].BusyScore, 
				IpAddress = servers[i].IpAddress, 
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
