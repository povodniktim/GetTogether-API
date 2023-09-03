using API.Services;
using API.Helpers;
using API.Models;

namespace API.Middlewares
{
    class TokenValidationMiddleware
    {

        private readonly RequestDelegate _next;

        public TokenValidationMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext httpContext, GetTogetherContext context)
        {
            if (httpContext.Request.Path.StartsWithSegments("/api/auth"))
            {
                await _next(httpContext);
                return;
            }

            string? accessToken = httpContext.Request.Query["access-token"];
            if (string.IsNullOrWhiteSpace(accessToken))
            {
                httpContext.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await httpContext.Response.WriteAsJsonAsync(new
                {
                    success = false,
                    message = "Unauthorized",
                    description = "Missing access token"
                });
                return;
            }

            if (TokenService.VerifyJWT(accessToken, EnvHelper.GetEnv(EnvVariable.JwtAccessTokenSecret)))
            {
                await _next(httpContext);
                return;
            }

            var refreshToken = httpContext.Request.Cookies["refreshToken"];
            if (string.IsNullOrWhiteSpace(refreshToken))
            {
                httpContext.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await httpContext.Response.WriteAsJsonAsync(new
                {
                    success = false,
                    message = "Unauthorized",
                    description = "Missing access token"
                });

                return;
            }

            var decryptedRefreshToken = CryptoService.Decrypt(refreshToken, EnvHelper.GetEnv(EnvVariable.EncryptionSecret));
            if (TokenService.VerifyJWT(decryptedRefreshToken, EnvHelper.GetEnv(EnvVariable.JwtRefreshTokenSecret)))
            {
                var userService = new UserService(context);
                var user = userService.GetByRefreshToken(refreshToken);
                if (user != null)
                {
                    var userPayload = UserService.GetTokenPayloadFromUser(user);
                    var tokens = TokenHelper.GenerateRefreshAndAccessTokens(userPayload, userPayload);
                    var encryptedRefreshToken = CryptoService.Encrypt(tokens.RefreshToken, EnvHelper.GetEnv(EnvVariable.EncryptionSecret));
                    userService.UpdateRefreshToken(user.Email, encryptedRefreshToken);

                    var cookieOptions = new CookieOptions
                    {
                        HttpOnly = true,
                        Secure = EnvHelper.IsProduction(),
                    };

                    httpContext.Response.Cookies.Append("refreshToken", encryptedRefreshToken, cookieOptions);
                    httpContext.Response.StatusCode = StatusCodes.Status200OK;
                    await httpContext.Response.WriteAsJsonAsync(new
                    {
                        success = false,
                        message = "Unauthorized",
                        accessToken = tokens.AccessToken
                    });

                    return;
                }
            }

            httpContext.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await httpContext.Response.WriteAsJsonAsync(new
            {
                success = false,
                message = "Unauthorized",
                callbackPath = "/auth/sign-in",
                description = "Access token has expired or is invalid",
            });
        }

    }
}