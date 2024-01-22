using API.Helpers;
using API.Models.Response.User;

namespace API.Models.Responses.Auth;

public class SignInResponse : GetUserResponse
{
    public TokenResponse tokens { get; set; }
}