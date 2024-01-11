using System.Text;
using System.Web;
using API.Helpers;
using API.Models;
using API.Models.Responses.Auth;

namespace API.Services;

public class AuthService
{
    private readonly UserService _userService;
    private readonly GetTogetherContext _context;

    public AuthService(GetTogetherContext context)
    {
        _context = context;
        _userService = new UserService(context);
    }

    public async Task<SignInResponse> SocialSignIn(UserSocialSignIn user, string provider)
    {
        bool isNewUser = false;

        User? targetUser = await _userService.GetByEmail(user.Email);
        if (targetUser == null)
        {
            isNewUser = true;
            targetUser = _userService.Create(
                 new User
                 {
                     Email = user.Email,
                     FirstName = user.FirstName,
                     LastName = user.LastName,
                     ProfileImageUrl = user.ProfileImageUrl,
                     GoogleId = (provider == "google") ? user.Id : null,
                     FacebookId = (provider == "facebook") ? user.Id : null,
                     AppleId = (provider == "apple") ? user.Id : null
                 }
             );
        }
        else if (
            (provider == "google" && (targetUser.GoogleId == null || targetUser.GoogleId != user.Id)) ||
            (provider == "facebook" && (targetUser.FacebookId == null || targetUser.FacebookId != user.Id)) ||
            (provider == "apple" && (targetUser.AppleId == null || targetUser.AppleId != user.Id))
        )
        {
            _userService.UpdateSocialId(user.Email, user.Id, provider);
        }

        var userPayload = ParseHelper.ParseObjToDictionary(UserService.GetTokenPayloadFromUser(targetUser));
        var tokens = TokenHelper.GenerateRefreshAndAccessTokens(userPayload, userPayload);

        var encryptedRefreshToken = CryptoService.Encrypt(tokens.RefreshToken, EnvHelper.GetEnv(EnvVariable.EncryptionSecret));
        var encryptedAccessToken = CryptoService.Encrypt(tokens.AccessToken, EnvHelper.GetEnv(EnvVariable.EncryptionSecret));

        await _userService.UpdateRefreshToken(user.Email, encryptedRefreshToken);

        return new SignInResponse
        {
            tokens = new TokenResponse
            {
                RefreshToken = HttpUtility.UrlEncode(encryptedRefreshToken),
                AccessToken = HttpUtility.UrlEncode(encryptedAccessToken)
            },
            Id = targetUser.Id,
            Email = targetUser.Email,
            FirstName = targetUser.FirstName,
            LastName = targetUser.LastName,
            ProfileImageUrl = targetUser.ProfileImageUrl,
            CreatedAt = targetUser.CreatedAt,
            CreatedEventsCount = isNewUser ? 0 : _context.Events.Count(e => e.OrganizerId == targetUser.Id),
            AttendedEventsCount = isNewUser ? 0 : _context.EventParticipants.Count(ep => ep.ParticipantId == targetUser.Id)
        };

    }

}