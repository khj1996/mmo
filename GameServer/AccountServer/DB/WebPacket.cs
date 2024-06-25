﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public class CreateAccountPacketReq
{
    public string AccountName { get; set; }
    public string Password { get; set; }
}

public class CreateAccountPacketRes
{
    public bool CreateOk { get; set; }
}

public class LoginAccountPacketReq
{
    public string AccountName { get; set; }
    public string Password { get; set; }
}

public class ServerInfo
{
    public string Name { get; set; }
    public string IpAddress { get; set; }
    public int Port { get; set; }
    public int BusyScore { get; set; }
}

public class LoginFacebookAccountPacketReq
{
    public string Token { get; set; }
}

public class LoginAccountPacketRes
{
    public bool LoginOk { get; set; }
    public string JwtAccessToken { get; set; }
    public string IpAddress { get; set; }
    public int Port { get; set; }
}