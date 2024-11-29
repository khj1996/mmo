using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PopupUIManager : Singleton<PopupUIManager>
{
    [Serializable]
    public class Popup
    {
        [SerializeField] public PopupUI UI;
        [SerializeField] public KeyCode key;
        [SerializeField] public PopupType type;
        public event Action<string> OnOpen;

        public void InvokeOnOpen(string data = null)
        {
            OnOpen?.Invoke(data);
        }
    }

    public enum PopupType
    {
        General,
        Special
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
        if (!Managers.GameStateManager.CanOpenPopUp())
            return;

        if (Input.GetKeyDown(_escapeKey))
        {
            if (_activePopupLList.Count > 0)
            {
                ClosePopup(_activePopupLList.First.Value);
            }
        }

        foreach (var popup in popList.Where(p => p.type == PopupType.General))
        {
            ToggleKeyDownAction(popup.key, popup);
        }
    }

    private void Init()
    {
        _allPopupList = popList.Select(x => x.UI).ToList();

        foreach (var popup in popList)
        {
            popup.UI.OnFocus += () =>
            {
                _activePopupLList.Remove(popup.UI);
                _activePopupLList.AddFirst(popup.UI);
                RefreshAllPopupDepth();
            };

            if (popup.UI.name == PopUpName.ShopUI)
            {
                popup.OnOpen += (data) =>
                {
                    var inventoryPopup = popList.FirstOrDefault(p => p.UI.name == PopUpName.InventoryUI);
                    if (inventoryPopup.UI != null && !inventoryPopup.UI.gameObject.activeSelf)
                    {
                        OpenPopup(inventoryPopup);
                    }
                };
            }
        }
    }


    private void InitCloseAll()
    {
        foreach (var popup in _allPopupList)
        {
            ClosePopup(popup);
        }
    }

    private void ToggleKeyDownAction(in KeyCode key, Popup popup)
    {
        if (Input.GetKeyDown(key))
            ToggleOpenClosePopup(popup);
    }

    private void ToggleOpenClosePopup(Popup popup)
    {
        if (!popup.UI.gameObject.activeSelf) OpenPopup(popup);
        else ClosePopup(popup.UI);
    }

    public void OpenPopupById(string popupName, string data = null)
    {
        var popup = popList.FirstOrDefault(p => p.UI.name == popupName);
        if (popup.UI == null)
        {
            Debug.LogWarning($"Popup with name {popupName} not found!");
            return;
        }

        OpenPopup(popup, data);
    }

    private void OpenPopup(Popup popup, string data = null)
    {
        _activePopupLList.AddFirst(popup.UI);
        popup.UI.gameObject.SetActive(true);
        popup.InvokeOnOpen(data);
        RefreshAllPopupDepth();
        UpdateGameState();
    }

    public void ClosePopup(PopupUI popup)
    {
        _activePopupLList.Remove(popup);
        popup.gameObject.SetActive(false);
        RefreshAllPopupDepth();
        UpdateGameState();
    }

    private void RefreshAllPopupDepth()
    {
        foreach (var popup in _activePopupLList)
        {
            popup.transform.SetAsFirstSibling();
        }
    }

    private void UpdateGameState()
    {
        if (_activePopupLList.Count > 0)
        {
            Managers.GameStateManager.SetState(GameState.InMenu);
        }
        else
        {
            Managers.GameStateManager.SetState(GameState.Normal);
        }
    }
}