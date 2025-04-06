using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace InternalApi.Utilitys
{
    public static class TokenUtility
    {
        public static string CreateJWTToken(string secretJWT, string issuerJWT, string audienceJWT, string userGuid, DateTime expirationTime)
        {
            if (string.IsNullOrEmpty(secretJWT) || string.IsNullOrEmpty(issuerJWT) || string.IsNullOrEmpty(audienceJWT) || !Guid.TryParse(userGuid, out var _))
            {
                throw new Exception("Пустые или не корректные данные!");
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretJWT));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim("userId", userGuid)
            };

            var token = new JwtSecurityToken(
                issuer: issuerJWT,
                audience: audienceJWT,
                claims: claims,
                expires: expirationTime,
                signingCredentials: creds);

            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

            if (tokenString == null)
            {
                throw new Exception("Не удалось создать токен!");
            }

            return tokenString;
        }
    }
}
