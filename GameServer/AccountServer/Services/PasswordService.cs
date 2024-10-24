﻿using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace AccountServer.Services
{
    public class PasswordService
    {
        IConfiguration _config;

        public PasswordService(IConfiguration config)
        {
            _config = config;
        }

        public string CreateJwtAccessToken(long accountDbId)
        {
            long now = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds();
            long expired = now + 60;

            var claims = new Claim[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, accountDbId.ToString()), // Subject
                new Claim(JwtRegisteredClaimNames.Iat, now.ToString()), // Issued At
                new Claim(JwtRegisteredClaimNames.Exp, expired.ToString()) // Expiration
            };

            var credentials = new SigningCredentials(
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes("dskfnglskjdnf;ogkjsndofignjdkfngolsjd")),
                SecurityAlgorithms.HmacSha256);

            var jwt = new JwtSecurityToken(claims: claims, signingCredentials: credentials);

            string token = new JwtSecurityTokenHandler().WriteToken(jwt);


            return token;
        }

        public JwtSecurityToken DecipherJwtAccessToken(string token)
        {
            JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();
            JwtSecurityToken decipher = handler.ReadJwtToken(token);
            return decipher;
        }

        public bool ValidateJwtAccessToken(string token, string key)
        {
            JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();

            TokenValidationParameters validationParams = new TokenValidationParameters()
            {
                ValidateLifetime = true,
                ValidateAudience = false,
                ValidateIssuer = false,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key))
            };

            try
            {
                var claims = handler.ValidateToken(token, validationParams, out SecurityToken validatedToken);
                return true;
            }
            catch
            {
                return false;
            }
        }

        // 비밀번호 해싱
        public static string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        // 비밀번호 검증
        public static bool VerifyPassword(string password, string hashedPassword)
        {
            return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
        }
    }
}