using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class PopupUIManager : MonoBehaviour
{
    [Serializable]
    public struct Popup
    {
        [SerializeField] public PopupUI UI;
        [SerializeField] public KeyCode key;
    }

    [Space] public KeyCode _escapeKey = KeyCode.Escape;

    public List<Popup> popList;

    private LinkedList<PopupUI> _activePopupLList;

    private List<PopupUI> _allPopupList;

    private void Awake()
    {
        _activePopupLList = new LinkedList<PopupUI>();
        Init();
    }

    private void Update()
    {
        if (Input.GetKeyDown(_escapeKey))
        {
            if (_activePopupLList.Count > 0)
            {
                ClosePopup(_activePopupLList.First.Value);
            }
        }

        foreach (var popup in popList)
        {
            ToggleKeyDownAction(popup.key, popup.UI);
        }
    }

    private void Init()
    {
        _allPopupList = popList.Select(x => x.UI).ToList();


        foreach (var popup in _allPopupList)
        {
            popup.OnFocus += () =>
            {
                _activePopupLList.Remove(popup);
                _activePopupLList.AddFirst(popup);
                RefreshAllPopupDepth();
            };

            //popup._closeButton.onClick.AddListener(() => ClosePopup(popup));
        }
    }

    private void InitCloseAll()
    {
        foreach (var popup in _allPopupList)
        {
            ClosePopup(popup);
        }
    }

    private void ToggleKeyDownAction(in KeyCode key, PopupUI popup)
    {
        if (Input.GetKeyDown(key))
            ToggleOpenClosePopup(popup);
    }

    private void ToggleOpenClosePopup(PopupUI popup)
    {
        if (!popup.gameObject.activeSelf) OpenPopup(popup);
        else ClosePopup(popup);
    }

    private void OpenPopup(PopupUI popup)
    {
        _activePopupLList.AddFirst(popup);
        popup.gameObject.SetActive(true);
        RefreshAllPopupDepth();
    }

    private void ClosePopup(PopupUI popup)
    {
        _activePopupLList.Remove(popup);
        popup.gameObject.SetActive(false);
        RefreshAllPopupDepth();
    }

    private void RefreshAllPopupDepth()
    {
        foreach (var popup in _activePopupLList)
        {
            popup.transform.SetAsFirstSibling();
        }
    }
}