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
            case UIEvent.BeginDrag:
                UI_DragHandler beginDragEvt = Util.GetOrAddComponent<UI_DragHandler>(go);
                beginDragEvt.OnBeginDragHandler -= action;
                beginDragEvt.OnBeginDragHandler += action;
                break;
            case UIEvent.EndDrag:
                UI_DragHandler endDragEvt = Util.GetOrAddComponent<UI_DragHandler>(go);
                endDragEvt.OnEndDragHandler -= action;
                endDragEvt.OnEndDragHandler += action;
                break;
        }
    }
}