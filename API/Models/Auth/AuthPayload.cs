namespace API.Models
{
    interface IUserTokenInfo
    {
        string? UserId { get; set; }
        string? Email { get; set; }
    }

    public class UserTokenInfo : IUserTokenInfo
    {
        public string? UserId { get; set; }
        public string? Email { get; set; }
    }

    interface IAuthTokenPayload : IJwtPayload, IUserTokenInfo { }

    public class AuthTokenPayload : JwtPayload, IAuthTokenPayload
    {
        public string? UserId { get; set; }
        public string? Email { get; set; }
    }

}