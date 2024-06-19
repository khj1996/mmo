using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AccountServer.DB;
using AccountServer.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AccountServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        AccountService _account;

        public AccountController(AccountService account)
        {
            _account = account;
        }

        [HttpPost]
        [Route("login/facebook")]
        public async Task<LoginAccountPacketRes> LoginAccountFB([FromBody] LoginFacebookAccountPacketReq req)
        {
            LoginAccountPacketRes res = new LoginAccountPacketRes();

            string jwtToken = await _account.LoginFacebookAccount(req.Token);
            if (string.IsNullOrEmpty(jwtToken))
            {
                res.LoginOk = false;
                return res;
            }

            res.LoginOk = true;
            res.JwtAccessToken = jwtToken;
            return res;
        }

        [HttpPost]
        [Route("create")]
        public CreateAccountPacketRes CreateAccount([FromBody] CreateAccountPacketReq req)
        {
            CreateAccountPacketRes res = new CreateAccountPacketRes();

            res.CreateOk = _account.CreateEmailAccount(req);

            return res;
        }

        [HttpPost]
        [Route("login")]
        public async Task<LoginAccountPacketRes> LoginAccount([FromBody] LoginAccountPacketReq req)
        {
            LoginAccountPacketRes res = new LoginAccountPacketRes();

            string jwtToken = await _account.LoginEmailAccount(req);
            if (string.IsNullOrEmpty(jwtToken))
            {
                res.LoginOk = false;
                return res;
            }

            res.LoginOk = true;
            res.JwtAccessToken = jwtToken;
            return res;
        }
    }
}