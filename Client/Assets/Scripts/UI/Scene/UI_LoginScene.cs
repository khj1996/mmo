using Facebook.Unity;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_LoginScene : UI_Scene
{
    public TMP_InputField AccountName;
    public TMP_InputField Password;

    public Button CreateBtn;
    public Button LoginBtn;


    public override void Init()
    {
        base.Init();       
        SceneName = "UI_LoginScene";


        CreateBtn.gameObject.BindEvent(OnClickCreateButton);
        LoginBtn.gameObject.BindEvent(OnClickLoginButton);
        
        isInitComplete = true;

       // FB.Init("", "");
    }

    public void OnClickCreateButton(PointerEventData evt)
    {
        string account = AccountName.text;
        string password = Password.text;

        CreateAccountPacketReq packet = new CreateAccountPacketReq()
        {
            AccountName = account,
            Password = password
        };

        Managers.Web.SendPostRequest<CreateAccountPacketRes>("account/create", packet, (res) =>
        {
            Debug.Log(res.CreateOk);
            AccountName.text = "";
            Password.text = "";
        });
    }

    public void OnClickLoginButton(PointerEventData evt)
    {
        Debug.Log("OnClickLoginButton");

        string account = AccountName.text;
        string password = Password.text;


        LoginAccountPacketReq packet = new LoginAccountPacketReq()
        {
            AccountName = account,
            Password = password
        };

        Managers.Web.SendPostRequest<LoginFacebookAccountPacketRes>("account/login", packet, (res) =>
        {
            Debug.Log(res.LoginOK);
            AccountName.text = "";
            Password.text = "";

            if (res.LoginOK)
            {
                Managers.Network.Token = res.JwtAccessToken;
                Managers.Network.Connect(new ServerInfo()
                {
                    Port = res.Port,
                    IpAddress = res.IpAddress
                });

                gameObject.SetActive(false);
            }
        });

        //FB.LogInWithPublishPermissions(new List<string>() { "email" }, OnFacebookResponse);
    }

    public void OnFacebookResponse(ILoginResult result)
    {
        Debug.Log("Connected FB");

        LoginFacebookAccountPacketReq packet = new LoginFacebookAccountPacketReq()
        {
            Token = result.AccessToken.TokenString
        };

        Managers.Web.SendPostRequest<LoginFacebookAccountPacketRes>("account/login/facebook", packet, (res) =>
        {
            Debug.Log(res.LoginOK);

            // TODO
        });
    }
}