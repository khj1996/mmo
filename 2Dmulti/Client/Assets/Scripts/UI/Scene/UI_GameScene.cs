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

    public UI_ItemNotice ItemNoticeUI { get; private set; }

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
        ItemNoticeUI = GetComponentInChildren<UI_ItemNotice>();

        StatUI.gameObject.SetActive(false);
        InvenUI.gameObject.SetActive(false);
        ShopUI.gameObject.SetActive(false);
        ExpUI.gameObject.SetActive(true);
        JoystickUI.gameObject.SetActive(true);
        ItemNoticeUI.gameObject.SetActive(true);

        InitButton();

        isInitComplete = true;
    }

    private void InitButton()
    {
        BindButton(btnInven, InvenUI, () => InvenUI.RefreshUI(Define.InvenRefreshType.All));
        BindButton(btnShop, ShopUI, ShopUI.RefreshUI);
        BindButton(btnCharacter, StatUI, StatUI.RefreshUI);
    }

    private void BindButton(Button button, UI_Base targetUI, Action refreshAction)
    {
        button.gameObject.BindEvent(_ =>
        {
            bool isActive = !targetUI.gameObject.activeSelf;
            targetUI.gameObject.SetActive(isActive);

            if (!isActive && refreshAction != null)
                refreshAction();
        });
    }
}