using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_Popup : UI_Base
{
    public Button CloseBtn;
    public TMP_Text Title;

    public override void Init()
    {
        Managers.UI.SetCanvas(gameObject, true);
    }

    public virtual void ClosePopupUI(PointerEventData evt)
    {
        Managers.UI.ClosePopupUI(this);
    }
}