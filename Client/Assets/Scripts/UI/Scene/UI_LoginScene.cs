using Facebook.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_LoginScene : UI_Scene
{
	enum GameObjects
	{
		AccountName,
		Password
	}

	enum Images
	{
		CreateBtn,
		LoginBtn
	}

    public override void Init()
	{
        base.Init();

		Bind<GameObject>(typeof(GameObjects));
		Bind<Image>(typeof(Images));

		GetImage((int)Images.CreateBtn).gameObject.BindEvent(OnClickCreateButton);
		GetImage((int)Images.LoginBtn).gameObject.BindEvent(OnClickLoginButton);

		FB.Init("2271097109703692", "1b54316978c0a5b6143ead29e921d92b");
	}

	public void OnClickCreateButton(PointerEventData evt)
	{
		string account = Get<GameObject>((int)GameObjects.AccountName).GetComponent<InputField>().text;
		string password = Get<GameObject>((int)GameObjects.Password).GetComponent<InputField>().text;

		CreateAccountPacketReq packet = new CreateAccountPacketReq()
		{
			AccountName = account,
			Password = password
		};

		Managers.Web.SendPostRequest<CreateAccountPacketRes>("account/create", packet, (res) =>
		{
			Debug.Log(res.CreateOk);
			Get<GameObject>((int)GameObjects.AccountName).GetComponent<InputField>().text = "";
			Get<GameObject>((int)GameObjects.Password).GetComponent<InputField>().text = "";
		});
	}

	public void OnClickLoginButton(PointerEventData evt)
	{
		Debug.Log("OnClickLoginButton");

		string account = Get<GameObject>((int)GameObjects.AccountName).GetComponent<InputField>().text;
		string password = Get<GameObject>((int)GameObjects.Password).GetComponent<InputField>().text;

		
		LoginAccountPacketReq packet = new LoginAccountPacketReq()
		{
			AccountName = account,
			Password = password
		};

		Managers.Web.SendPostRequest<LoginFacebookAccountPacketRes>("account/login", packet, (res) =>
		{
			Debug.Log(res.LoginOK);
			Get<GameObject>((int)GameObjects.AccountName).GetComponent<InputField>().text = "";
			Get<GameObject>((int)GameObjects.Password).GetComponent<InputField>().text = "";

			if (res.LoginOK)
			{
				Managers.Network.AccountId = res.AccountId;
				Managers.Network.Token = res.Token;

				UI_SelectServerPopup popup = Managers.UI.ShowPopupUI<UI_SelectServerPopup>();
				popup.SetServers(res.ServerList);
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
