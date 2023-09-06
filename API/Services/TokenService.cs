using API.Helpers;
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

    }
}
