using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_GameScene : UI_Scene
{
    public UI_Stat StatUI { get; private set; }
    public UI_Inventory InvenUI { get; private set; }
    public UI_Shop ShopUI { get; private set; }
    public UI_GetItemPopUp GetItemPopUp { get; private set; }

    public override void Init()
    {
        base.Init();

        StatUI = GetComponentInChildren<UI_Stat>();
        InvenUI = GetComponentInChildren<UI_Inventory>();
        ShopUI = GetComponentInChildren<UI_Shop>();
        GetItemPopUp = GetComponentInChildren<UI_GetItemPopUp>();

        StatUI.gameObject.SetActive(false);
        InvenUI.gameObject.SetActive(false);
        ShopUI.gameObject.SetActive(false);
        GetItemPopUp.gameObject.SetActive(false);
    }
}