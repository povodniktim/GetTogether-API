using API.Helpers;
using API.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace API.Services
{
    public static class TokenService
    {
        public static string GenerateJWT(Dictionary<string, string> customClaims, string secret, int expiresAtMinutes)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var securityKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(secret));

            var tokenClaims = new List<Claim>
                {
                    new Claim(JwtRegisteredClaimNames.Iss, EnvHelper.GetEnv(EnvVariable.JwtIssuer)),
                    new Claim(JwtRegisteredClaimNames.Aud, EnvHelper.GetEnv(EnvVariable.JwtAudience)),
                };

            foreach (var claim in customClaims)
            {
                tokenClaims.Add(new Claim(claim.Key, claim.Value));
            }

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Expires = DateTime.UtcNow.AddMinutes(expiresAtMinutes),
                SigningCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256),
                Subject = new ClaimsIdentity(tokenClaims)
            };

            return tokenHandler.WriteToken(tokenHandler.CreateToken(tokenDescriptor));
        }

        public static bool VerifyJWT(string token, string secret)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var securityKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(secret));

                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = securityKey,
                    ValidIssuer = EnvHelper.GetEnv(EnvVariable.JwtIssuer),
                    ValidAudience = EnvHelper.GetEnv(EnvVariable.JwtAudience),
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                return true;
            }
            catch (Exception)
            {
                return false;
            }

        }

        public static AuthTokenPayload GetAuthTokenPayload(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtSecurityToken = tokenHandler.ReadJwtToken(token);

            var claims = jwtSecurityToken.Claims
                .Where(claim => !string.IsNullOrEmpty(claim.Type))
                .ToDictionary(claim => claim.Type, claim => claim.Value);

            return new AuthTokenPayload
            {
                UserId = claims.ContainsKey("UserId") ? claims["UserId"] : null,
                Email = claims.ContainsKey("Email") ? claims["Email"] : null,
                Issuer = claims.ContainsKey("iss") ? claims["iss"] : null,
                Audience = claims.ContainsKey("aud") ? claims["aud"] : null,
                IssuedAt = DateTime.UnixEpoch.AddSeconds(claims.ContainsKey("iat") ? Convert.ToDouble(claims["iat"]) : 0),
                Expiration = DateTime.UnixEpoch.AddSeconds(claims.ContainsKey("exp") ? Convert.ToDouble(claims["exp"]) : 0),
                Subject = claims.ContainsKey("sub") ? claims["sub"] : null
            };
        }

    }
}
