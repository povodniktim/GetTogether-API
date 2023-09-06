using API.Helpers;
using API.Models;
using API.Services;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers.Auth
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : Controller
    {
        private readonly UserService _userService;
        private readonly GetTogetherContext _context;

        public AuthController(GetTogetherContext context)
        {
            _context = context;
            _userService = new UserService(_context);
        }

        [HttpPost("sign-in")]
        public async Task<ActionResult<User>> SignIn(User user)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var targetUser = _userService.GetByEmail(user.Email);
            if (targetUser == null)
            {
                targetUser = _userService.Create(user);
            }

            var userPayload = ParseHelper.ParseObjToDictionary(UserService.GetTokenPayloadFromUser(targetUser));

            var tokens = TokenHelper.GenerateRefreshAndAccessTokens(userPayload, userPayload);

            var encryptedRefreshToken = CryptoService.Encrypt(tokens.RefreshToken, EnvHelper.GetEnv(EnvVariable.EncryptionSecret));

            _userService.UpdateRefreshToken(targetUser.Email, encryptedRefreshToken);

            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = EnvHelper.IsProduction()
            };

            Response.Cookies.Append("refreshToken", encryptedRefreshToken, cookieOptions);

            return Ok(new
            {
                success = true,
                accessToken = tokens.AccessToken
            });
        }

        [HttpGet("sign-out")]
        public async Task<ActionResult<dynamic>> SignOut()
        {
            string? accessToken = HttpContext.Request.Query["access-token"];
            if (string.IsNullOrWhiteSpace(accessToken))
            {
                return BadRequest(new
                {
                    success = false,
                    message = "Access token is required"
                });
            }

            var userPayload = TokenService.GetAuthTokenPayload(accessToken);
            _userService.UpdateRefreshToken(userPayload.Email, null);

            Response.Cookies.Delete("refreshToken");

            return Ok(new
            {
                success = true,
                message = "Signed out successfully"
            });

        }
    }

}