using Microsoft.IdentityModel.Tokens;
using TokenAuthAPI.Models;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;

namespace TokenAuthAPI.Services
{
    public static class TokenService
    {
        public static string GenerateToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();

            var key = Encoding.ASCII.GetBytes(Settings.Secret);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, user.Username),  // mapeado para User.Identity.Name do ASP.NET
                    new Claim("Email", user.Email),
                    new Claim(ClaimTypes.Role, user.Role)
                }),
                Expires = DateTime.UtcNow.AddMinutes(30),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }

        public static string GenerateToken(IEnumerable<Claim> claims)
        {
            var tokenHandler = new JwtSecurityTokenHandler();

            var key = Encoding.ASCII.GetBytes(Settings.Secret);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(30),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }

        public static string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }

        public static ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();

            var key = Encoding.ASCII.GetBytes(Settings.Secret);

            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = false
            };

            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out var securityToken);

            //if (securityToken is not JwtSecurityToken jwtSecurityToken || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))  // C# 9.0
            if (securityToken == null || !((JwtSecurityToken)securityToken).Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))  // C# 8.0 ou anteriores
                throw new SecurityTokenException("Token inválido.");

            return principal;
        }

        //private static List<(string, string)> _refreshTokens = new();  // C# 9.0
        private static List<(string, string)> _refreshTokens = new List<(string, string)>();  // C# 8.0 ou anteriores

        public static void SaveRefreshToken(string username, string refreshToken)
        {
            //_refreshTokens.Add(new(username, refreshToken));  // C# 9.0
            _refreshTokens.Add((username, refreshToken));  // C# 8.0 ou anteriores
        }

        public static string GetRefreshToken(string username)
        {
            return _refreshTokens.FirstOrDefault(x => x.Item1 == username).Item2;
        }

        public static void DeleteRefreshToken(string username, string refreshToken)
        {
            var item = _refreshTokens.FirstOrDefault(x => x.Item1 == username && x.Item2 == refreshToken);

            _refreshTokens.Remove(item);
        }
    }
}
