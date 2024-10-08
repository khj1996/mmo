using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameScene : MonoBehaviour
{
    UI_GameScene _sceneUI;

    private void Start()
    {
        Init();
    }

    protected async void Init()
    {
        Managers.Map.LoadMap(1);

        Screen.SetResolution(1280, 720, false);

        _sceneUI = await Managers.UI.ShowSceneUI<UI_GameScene>();
    }

    public void Clear()
    {
    }
}