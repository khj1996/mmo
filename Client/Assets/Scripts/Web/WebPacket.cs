using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateAccountPacketReq
{
	public string AccountName;
	public string Password;
}

public class CreateAccountPacketRes
{
	public bool CreateOk;
}

public class LoginAccountPacketReq
{
	public string AccountName;
	public string Password;
}


public class ServerInfo
{
	public string Name;
	public string IpAddress;
	public int Port;
	public int BusyScore;
}

public class LoginFacebookAccountPacketReq
{
	public string Token;
}

public class LoginFacebookAccountPacketRes
{
	public bool LoginOK;
	public string JwtAccessToken;
}