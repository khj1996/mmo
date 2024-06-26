using AccountServer.DB;
using AccountServer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace AccountServer.Services
{
    public class AccountService
    {
        AppDbContext _context;
        FacebookService _fb;
        JwtTokenService _token;

        public AccountService(AppDbContext context, FacebookService fb, JwtTokenService token)
        {
            _context = context;
            _fb = fb;
            _token = token;
        }

        public bool CreateEmailAccount(CreateAccountPacketReq packet)
        {
            AccountDb account = _context.Accounts
                .AsNoTracking()
                .FirstOrDefault(a => a.LoginProviderUserId == packet.AccountName);

            if (account == null)
            {
                _context.Accounts.Add(new AccountDb()
                {
                    LoginProviderUserId = packet.AccountName,
                    LoginProviderType = ProviderType.Email
                });

                bool success = _context.SaveChangesEx();
                return success;
            }

            return false; 
        }


        public async Task<string> LoginEmailAccount(LoginAccountPacketReq packet)
        {
            if (packet == null)
                return null;

            AccountDb accountDb = _context.Accounts.FirstOrDefault(
                a => a.LoginProviderUserId == packet.AccountName
                     && a.LoginProviderType == ProviderType.Email);

            if (accountDb == null)
            {
                return null;
            }

            string jwtToken = _token.CreateJwtAccessToken(accountDb.AccountDbId);
            return jwtToken;
        }


        public async Task<string> LoginFacebookAccount(string token)
        {
            FacebookTokenData tokenData = await _fb.GetUserTokenData(token);
            if (tokenData == null || tokenData.is_valid == false)
                return null;

            AccountDb accountDb = _context.Accounts.FirstOrDefault(
                a => a.LoginProviderUserId == tokenData.user_id
                     && a.LoginProviderType == ProviderType.Facebook);

            if (accountDb == null)
            {
                accountDb = new AccountDb()
                {
                    LoginProviderUserId = tokenData.user_id,
                    LoginProviderType = ProviderType.Facebook
                };

                _context.Accounts.Add(accountDb);
                await _context.SaveChangesAsync();
            }

            string jwtToken = _token.CreateJwtAccessToken(accountDb.AccountDbId);
            return jwtToken;
        }
    }
}