namespace API.Models
{
    interface IJwtPayload
    {
        string? Issuer { get; set; }
        string? Audience { get; set; }
        DateTime? IssuedAt { get; set; }
        DateTime? Expiration { get; set; }
        string? Subject { get; set; }
    }

    public class JwtPayload
    {
        public string? Issuer { get; set; }
        public string? Audience { get; set; }
        public DateTime? IssuedAt { get; set; }
        public DateTime? Expiration { get; set; }
        public string? Subject { get; set; }
    }
}