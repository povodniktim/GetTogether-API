using API.Services;
using API.Helpers;
using API.Models;
using API.Responses;

namespace API.Middlewares
{
    class TokenValidationMiddleware
    {
        private readonly string[] _pathsToSkip = { "/api/auth/sign-in" };
        private readonly RequestDelegate _next;

        public TokenValidationMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext httpContext, GetTogetherContext context)
        {
            if (_pathsToSkip.Any(path => httpContext.Request.Path.StartsWithSegments(path)))
            {
                await _next(httpContext);
                return;
            }

            string? accessToken = httpContext.Request.Query["access-token"];
            if (string.IsNullOrWhiteSpace(accessToken))
            {
                httpContext.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await httpContext.Response.WriteAsJsonAsync(
                    new ErrorResponse<string>(
                        new string[] { "Missing access token" },
                        "Unauthorized"
                    )
                );
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
                await httpContext.Response.WriteAsJsonAsync(
                    new ErrorResponse<string>(
                        new string[] { "Missing access token" },
                        "Unauthorized"
                    )
                );

                return;
            }

            var decryptedRefreshToken = CryptoService.Decrypt(refreshToken, EnvHelper.GetEnv(EnvVariable.EncryptionSecret));
            if (TokenService.VerifyJWT(decryptedRefreshToken, EnvHelper.GetEnv(EnvVariable.JwtRefreshTokenSecret)))
            {
                var userService = new UserService(context);
                var user = userService.GetByRefreshToken(refreshToken);
                if (user != null)
                {
                    var userPayload = ParseHelper.ParseObjToDictionary(UserService.GetTokenPayloadFromUser(user));
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
                    await httpContext.Response.WriteAsJsonAsync(
                        new ErrorResponse<AuthResponse>(
                            new string[] { "Access token has expired" },
                            "Unauthorized",
                            new AuthResponse(tokens.AccessToken)
                        )
                    );

                    return;
                }
            }

            httpContext.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await httpContext.Response.WriteAsJsonAsync(
                new ErrorResponse<string>(
                    new string[] { "Access token has expired or is invalid" },
                    "Unauthorized"
                )
            );
        }

    }
}