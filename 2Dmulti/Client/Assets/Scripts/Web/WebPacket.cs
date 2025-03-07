﻿using System.Collections;
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

public class CharacterInfo
{
    public string PlayerName;
    public int Lv;
    public int CurMap;
    public float PosX;
    public float PosY;
}


public class LoginFacebookAccountPacketReq
{
    public string Token;
}

public class LoginAccountPacketRes
{
    public bool LoginOK;
    public string JwtAccessToken;
    public string IpAddress;
    public int Port;
}