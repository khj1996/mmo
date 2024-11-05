using System;
using UnityEngine;
using UnityEngine.UI;

public class UI_GameScene : UI_Scene
{
    public UI_Stat StatUI { get; private set; }
    public UI_Inventory InvenUI { get; private set; }
    public UI_Shop ShopUI { get; private set; }
    public UI_ExpBar ExpUI { get; private set; }
    public UI_Joystick JoystickUI { get; private set; }

    public Button btnShop;
    public Button btnInven;
    public Button btnCharacter;

    public override void Init()
    {
        base.Init();
        SceneName = "UI_GameScene";

        StatUI = GetComponentInChildren<UI_Stat>();
        InvenUI = GetComponentInChildren<UI_Inventory>();
        ShopUI = GetComponentInChildren<UI_Shop>();
        ExpUI = GetComponentInChildren<UI_ExpBar>();
        JoystickUI = GetComponentInChildren<UI_Joystick>();

        SetUIActive(StatUI, false);
        SetUIActive(InvenUI, false);
        SetUIActive(ShopUI, false);
        SetUIActive(ExpUI, true);
        SetUIActive(JoystickUI, true);

        InitButton();

        isInitComplete = true;
    }

    private void InitButton()
    {
        BindButton(btnInven, InvenUI, () => InvenUI.RefreshUI(Define.InvenRefreshType.All));
        BindButton(btnShop, ShopUI, ShopUI.RefreshUI);
        BindButton(btnCharacter, StatUI, StatUI.RefreshUI);
    }

    private void SetUIActive(MonoBehaviour uiElement, bool isActive)
    {
        if (uiElement != null)
            uiElement.gameObject.SetActive(isActive);
    }

    private void BindButton(Button button, MonoBehaviour uiElement, Action refreshAction)
    {
        button.gameObject.BindEvent(_ =>
        {
            bool isActive = uiElement.gameObject.activeSelf;
            SetUIActive(uiElement, !isActive);

            if (!isActive && refreshAction != null)
                refreshAction();
        });
    }
}