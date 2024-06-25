using System.IdentityModel.Tokens.Jwt;
using System.Text;
using LoginServer.DB;
using Microsoft.IdentityModel.Tokens;

public static class Extensions
{
    public static bool SaveChangesEx(this AppDbContext db)
    {
        try
        {
            db.SaveChanges();
            return true;
        }
        catch
        {
            return false;
        }
    }
}


public static class JwtUtils
{
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