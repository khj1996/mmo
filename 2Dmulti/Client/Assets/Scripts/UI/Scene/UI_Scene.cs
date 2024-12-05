using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_Scene : UI_Base
{
	public bool isInitComplete = false;
	public string SceneName;
	
	public override void Init()
	{
		isInitComplete = false;
		Managers.UI.SetCanvas(gameObject, false);
	}
}
