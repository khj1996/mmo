using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoginScene : MonoBehaviour
{
    UI_LoginScene _sceneUI;

    private void Start()
    {
        Init();
    }

    protected async void Init()
    {
        Screen.SetResolution(640, 480, false);

        _sceneUI = await Managers.UI.ShowSceneUI<UI_LoginScene>();
    }

    public void Clear()
    {
    }
}