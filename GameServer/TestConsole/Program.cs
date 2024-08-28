using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace AccountServer
{
    public class Program
    {
        public static int Main(string[] args)
        {
            Console.WriteLine(Math.Truncate(0.5f));
            Console.WriteLine(Math.Truncate(-0.5f));

            return 0;
            Stopwatch sw = new Stopwatch();

            Random rand = new Random();

            double sum = 0.0;

            int cnt = 0;


            var num = 0.0f;
            for (int i = 0; i < 100000000; i++)
            {
                /*var first = rand.Next();
                var send = rand.Next();*/

                var first = rand.NextDouble();
                var send = rand.NextDouble();
                long startTicks = DateTime.UtcNow.Ticks;

                var test = Math.Abs(first - send) < 0.0f;

                long endTicks = DateTime.UtcNow.Ticks;


                // 측정된 시간 출력
                long elapsedTicks = endTicks - startTicks;
                TimeSpan elapsedSpan = new TimeSpan(elapsedTicks);
                sum += elapsedSpan.TotalMilliseconds;
            }

            Console.WriteLine("측정된 시간: {0}ms", sum);


            /*
            long now = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds();
            long expired = now -1000;

            var claims = new Claim[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, "435634"), // Subject
                new Claim(JwtRegisteredClaimNames.Iat, now.ToString()), // Issued At
                new Claim(JwtRegisteredClaimNames.Exp, expired.ToString()) // Expiration
            };


            var credentials = new SigningCredentials(
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes("dskfnglskjdnf;ogkjsndofignjdkfngolsjd")),
                SecurityAlgorithms.HmacSha256);

            var jwt = new JwtSecurityToken(claims: claims, signingCredentials: credentials);

            string token = new JwtSecurityTokenHandler().WriteToken(jwt);

            Console.Write(token);

            var test1 = DecipherJwtAccessToken(token);
            var test2 = ValidateJwtAccessToken(token, "dskfnglskjdnf;ogkjsndofignjdkfngolsjd");

            return int.Parse(test1.Subject);*/
            return 0;
        }


        public static JwtSecurityToken DecipherJwtAccessToken(string token)
        {
            JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();
            JwtSecurityToken decipher = handler.ReadJwtToken(token);
            return decipher;
        }

        public static bool ValidateJwtAccessToken(string token, string key)
        {
            JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();

            TokenValidationParameters validationParams = new TokenValidationParameters()
            {
                ValidateLifetime = true,
                ValidateAudience = false,
                ValidateIssuer = false,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key))
            };

            SecurityToken validatedToken;
            try
            {
                var claims = handler.ValidateToken(token, validationParams, out validatedToken);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}