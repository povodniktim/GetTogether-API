using API.Helpers;
using API.Models;
using API.Models.Response.User;
using API.Models.Responses.Auth;
using API.Responses;
using API.Services;
using Microsoft.AspNetCore.Mvc;
using System.Web;

namespace API.Controllers.Auth
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : Controller
    {
        private readonly string[] supportedProviders = { "google" };
        private readonly AuthService _authService;
        private readonly UserService _userService;
        private readonly GetTogetherContext _context;

        public AuthController(GetTogetherContext context)
        {
            _context = context;
            _authService = new AuthService(_context);
            _userService = new UserService(_context);
        }

        [HttpPost("social")]
        public async Task<ActionResult<User>> SocialSignIn
        (
            [FromQuery] string? provider,
            [FromBody] UserSocialSignIn user
        )
        {

            try
            {
                if (!ModelState.IsValid)
                {
                    throw new Exception("INVALID_USER_DATA");
                }

                if (provider == null || !supportedProviders.Contains(provider))
                {
                    throw new Exception("INVALID SOCIAL PROVIDER");
                }

                return Ok(
                    new SuccessResponse<SignInResponse>(
                       await _authService.SocialSignIn(user, "google")
                    )
                );
            }
            catch (Exception e)
            {
                return BadRequest
                (
                    new ErrorResponse<object>
                    (
                        new string[] { e.Message },
                        "Sign up has failed"
                    )
                );
            }

        }

        [HttpGet("sign-out")]
        public new async Task<IActionResult> SignOut()
        {
            try
            {
                string accessToken = TokenHelper.GetBearerTokenFromHeader(Request.Headers["Authorization"]);

                var decodedAccessToken = HttpUtility.UrlDecode(accessToken);

                var decryptedAccessToken = CryptoService.Decrypt(
                    decodedAccessToken,
                    EnvHelper.GetEnv(EnvVariable.EncryptionSecret)
                );

                if (!TokenService.VerifyJWT(
                        decryptedAccessToken,
                        EnvHelper.GetEnv(EnvVariable.JwtAccessTokenSecret)
                    ))
                {
                    throw new Exception("INVALID_ACCESS_TOKEN");
                }

                AuthTokenPayload payload = TokenService.GetAuthTokenPayload(decryptedAccessToken);

                if (payload.UserId == null || payload.Email == null)
                {
                    throw new Exception("INVALID_ACCESS_TOKEN");
                }

                User? user = await _userService.GetByEmail(payload.Email);

                if (user == null)
                {
                    throw new Exception("USER_NOT_FOUND");
                }

                await _userService.UpdateRefreshToken(user.Email, null);

                return Ok
                (
                    new SuccessResponse<string>
                    (
                        "Sign out successfully"
                    )
                );

            }
            catch (Exception e)
            {
                return BadRequest
                (
                    new ErrorResponse<string>
                    (
                        new string[] { e.Message },
                        "Sign out has failed"
                    )
                );
            }

        }

        [HttpGet("me")]
        public async Task<IActionResult> Me()
        {
            try
            {
                string encryptedAccessToken = TokenHelper.GetBearerTokenFromHeader(Request.Headers["Authorization"]);

                if (string.IsNullOrWhiteSpace(encryptedAccessToken))
                {
                    throw new Exception("ACCESS_TOKEN_REQUIRED");
                }

                var decodedAccessToken = HttpUtility.UrlDecode(encryptedAccessToken);
                var decryptedAccessToken = CryptoService.Decrypt(
                    decodedAccessToken,
                    EnvHelper.GetEnv(EnvVariable.EncryptionSecret)
                );

                if (!TokenService.VerifyJWT(
                    decryptedAccessToken,
                    EnvHelper.GetEnv(EnvVariable.JwtAccessTokenSecret)
                ))
                {
                    throw new Exception("INVALID_ACCESS_TOKEN");
                }

                string? email = TokenService.GetAuthTokenPayload(decryptedAccessToken).Email;

                if (email == null)
                {
                    throw new Exception("EMAIL_NOT_FOUND");
                }

                User? user = await _userService.GetByEmail(email);
                if (user == null || user.RefreshToken == null)
                {
                    throw new Exception("USER_NOT_FOUND");
                }

                GetUserResponse userResponse = new GetUserResponse()
                {
                    Id = user.Id,
                    Email = user.Email,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    ProfileImageUrl = user.ProfileImageUrl,
                    CreatedAt = user.CreatedAt,
                    CreatedEventsCount = _context.Events.Count(e => e.OrganizerId == user.Id),
                    AttendedEventsCount = _context.EventParticipants.Count(ep => ep.ParticipantId == user.Id),
                    NotificationsCount = _context.Notifications.Count(n => n.OrganizerId == user.Id)
                };

                return Ok
                (
                    new SuccessResponse<GetUserResponse>
                    (
                        userResponse,
                        "User found"
                    )
                );

            }
            catch (Exception e)
            {
                if (e.Message == "INVALID_ACCESS_TOKEN" || e.Message == "USER_NOT_FOUND")
                {
                    return Unauthorized
                    (
                        new ErrorResponse<string>
                        (
                            new string[] { e.Message },
                            "Invalid access token"
                        )
                    );
                }

                return BadRequest
                (
                    new ErrorResponse<string>
                    (
                        new string[] { e.Message },
                        "User not found"
                    )
                );
            }

        }

        [HttpPost("refresh")]
        public async Task<IActionResult> RefreshToken()
        {
            try
            {
                string refreshToken = TokenHelper.GetBearerTokenFromHeader(Request.Headers["Authorization"]);

                if (refreshToken == null || string.IsNullOrWhiteSpace(refreshToken))
                {
                    throw new Exception("INVALID_REFRESH_TOKEN");
                }

                var decodedRefreshToken = HttpUtility.UrlDecode(refreshToken);

                var decryptedRefreshToken = CryptoService.Decrypt(
                    decodedRefreshToken,
                    EnvHelper.GetEnv(EnvVariable.EncryptionSecret)
                );

                if (!TokenService.VerifyJWT(
                        decryptedRefreshToken,
                        EnvHelper.GetEnv(EnvVariable.JwtRefreshTokenSecret)
                    ))
                {
                    throw new Exception("INVALID_REFRESH_TOKEN");
                }

                AuthTokenPayload payload = TokenService.GetAuthTokenPayload(decryptedRefreshToken);
                if (payload.UserId == null || payload.Email == null)
                {
                    throw new Exception("INVALID_REFRESH_TOKEN");
                }

                User? user = await _userService.GetByEmail(payload.Email);
                if (user == null)
                {
                    throw new Exception("USER_NOT_FOUND");
                }


                if (user.RefreshToken != decodedRefreshToken)
                {
                    throw new Exception("INVALID_REFRESH_TOKEN");
                }

                var userPayload = ParseHelper.ParseObjToDictionary(UserService.GetTokenPayloadFromUser(user));
                var tokens = TokenHelper.GenerateRefreshAndAccessTokens(userPayload, userPayload);

                var encryptedRefreshToken = CryptoService.Encrypt(tokens.RefreshToken, EnvHelper.GetEnv(EnvVariable.EncryptionSecret));
                var encryptedAccessToken = CryptoService.Encrypt(tokens.AccessToken, EnvHelper.GetEnv(EnvVariable.EncryptionSecret));

                await _userService.UpdateRefreshToken(user.Email, encryptedRefreshToken);

                return Ok
                (
                    new SuccessResponse<TokenResponse>
                    (
                        new TokenResponse()
                        {
                            AccessToken = HttpUtility.UrlEncode(encryptedAccessToken),
                            RefreshToken = HttpUtility.UrlEncode(encryptedRefreshToken)
                        },
                        "Token refreshed successfully"
                    )
                );

            }
            catch (Exception e)
            {
                return BadRequest
                (
                    new ErrorResponse<string>
                    (
                        new string[] { e.Message },
                        "Invalid refresh token"
                    )
                );
            }

        }

    }

}