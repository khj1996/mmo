using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public abstract class UI_Base : MonoBehaviour
{
    public abstract void Init();

    private void Start()
    {
        Init();
    }


    public static void BindEvent(GameObject go, Action<PointerEventData> action, UIEvent type = UIEvent.Click)
    {
        switch (type)
        {
            case UIEvent.Click:
                UI_ClickHandler clickEvt = Util.GetOrAddComponent<UI_ClickHandler>(go);
                clickEvt.OnClickHandler -= action;
                clickEvt.OnClickHandler += action;
                break;
            case UIEvent.Drag:
                UI_DragHandler dragEvt = Util.GetOrAddComponent<UI_DragHandler>(go);
                dragEvt.OnDragHandler -= action;
                dragEvt.OnDragHandler += action;
                break;
        }
    }
}