using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_GameScene : UI_Scene
{
    public UI_Stat StatUI { get; private set; }
    public UI_Inventory InvenUI { get; private set; }
    public UI_Shop ShopUI { get; private set; }
    public UI_ExpBar ExpUI { get; private set; }
    public UI_Joystick JoystickUI { get; private set; }

    public override void Init()
    {
        base.Init();
        SceneName = "UI_GameScene";

        StatUI = GetComponentInChildren<UI_Stat>();
        InvenUI = GetComponentInChildren<UI_Inventory>();
        ShopUI = GetComponentInChildren<UI_Shop>();
        ExpUI = GetComponentInChildren<UI_ExpBar>();
        JoystickUI = GetComponentInChildren<UI_Joystick>();

        StatUI.gameObject.SetActive(false);
        InvenUI.gameObject.SetActive(false);
        ShopUI.gameObject.SetActive(false);
        ExpUI.gameObject.SetActive(true);
        JoystickUI.gameObject.SetActive(true);
        isInitComplete = true;
    }
}