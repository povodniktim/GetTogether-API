using API.Services;

namespace API.Helpers
{
    public class TokenResponse
    {
        public string RefreshToken { get; set; }
        public string AccessToken { get; set; }
    }

    public static class TokenHelper
    {
        public static TokenResponse GenerateRefreshAndAccessTokens(Dictionary<string, string> refreshPayload, Dictionary<string, string> accessPayload)
        {
            string RefreshToken = TokenService.GenerateJWT(
               refreshPayload,
               EnvHelper.GetEnv(EnvVariable.JwtRefreshTokenSecret),
               int.Parse(EnvHelper.GetEnv(EnvVariable.JwtRefreshTokenExpirationMinutes))
           );

            string AccessToken = TokenService.GenerateJWT(
                accessPayload,
                EnvHelper.GetEnv(EnvVariable.JwtAccessTokenSecret),
                int.Parse(EnvHelper.GetEnv(EnvVariable.JwtAccessTokenExpirationMinutes))
            );

            return new TokenResponse
            {
                RefreshToken = RefreshToken,
                AccessToken = AccessToken
            };
        }
        public static string GetBearerTokenFromHeader(string? header)
        {

            if (header == null || !header.StartsWith("Bearer "))
            {
                throw new Exception("Invalid bearer token");
            }

            return header.Substring("Bearer ".Length).Trim();
        }

    }

}