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
        PasswordService _token;

        public AccountService(AppDbContext context, FacebookService fb, PasswordService token)
        {
            _context = context;
            _fb = fb;
            _token = token;
        }

        public bool CreateEmailAccount(CreateAccountPacketReq packet)
        {
            var hashedPassword = PasswordService.HashPassword(packet.Password);

            var account = _context.Accounts
                .AsNoTracking()
                .FirstOrDefault(a => a.AccountName == packet.AccountName && a.Password == packet.Password);

            if (account == null)
            {
                _context.Accounts.Add(new AccountDb()
                {
                    AccountName = packet.AccountName,
                    Password = hashedPassword,
                    LoginProviderType = ProviderType.Email,
                });

                bool success = _context.SaveChangesEx();
                return success;
            }

            return false;
        }


        public string? LoginEmailAccount(LoginAccountPacketReq? packet)
        {
            if (packet == null)
                return null;

            var accountDb = _context.Accounts.FirstOrDefault(
                a => a.AccountName == packet.AccountName
                     && a.LoginProviderType == ProviderType.Email);

            //비밀번호가 틀리거나 계정이 없을시
            if (accountDb == null || !PasswordService.VerifyPassword(packet.Password, accountDb.Password))
            {
                return null;
            }


            var jwtToken = _token.CreateJwtAccessToken(accountDb.AccountDbId);
            return jwtToken;
        }


        public async Task<string> LoginFacebookAccount(string token)
        {
            var tokenData = await _fb.GetUserTokenData(token);
            if (tokenData == null || tokenData.is_valid == false)
                return null;

            var accountDb = _context.Accounts.FirstOrDefault(
                a => a.AccountName == tokenData.user_id
                     && a.LoginProviderType == ProviderType.Facebook);

            if (accountDb == null)
            {
                accountDb = new AccountDb()
                {
                    AccountName = tokenData.user_id,
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