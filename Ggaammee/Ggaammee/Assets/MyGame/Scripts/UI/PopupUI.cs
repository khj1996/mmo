using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class PopupUI : MonoBehaviour, IPointerDownHandler
{
    //public Button _closeButton;
    public event Action OnFocus;

    public void OnPointerDown(PointerEventData eventData)
    {
        OnFocus?.Invoke(); 
    }
}