using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_SelectServerPopup_Item : UI_Base
{
	public ServerInfo Info { get; set; }

	enum Buttons
	{
		SelectServerButton
	}

	enum TMP_Texts
	{ 
		NameText
	}

	public override void Init()
	{
		Bind<Button>(typeof(Buttons));
		Bind<TMP_Text>(typeof(TMP_Texts));

		GetButton((int)Buttons.SelectServerButton).gameObject.BindEvent(OnClickButton);
	}

	public void RefreshUI()
	{
		if (Info == null)
			return;

		GetTMP((int)TMP_Texts.NameText).text = Info.Name;
	}

	void OnClickButton(PointerEventData evt)
	{
		Managers.Network.Connect(Info);
		Managers.UI.ClosePopupUI();
	}
}
