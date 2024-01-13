using API.Models;
using Microsoft.EntityFrameworkCore;

namespace API.Services
{
    public class UserService
    {
        private readonly GetTogetherContext _context;

        public UserService(GetTogetherContext context)
        {
            _context = context;
        }

        public async Task<User?> GetByEmail(string email)
        {
            return await _context.Users.FirstOrDefaultAsync(user => user.Email == email);
        }

        public User? GetByRefreshToken(string token)
        {
            return _context.Users.FirstOrDefault(user => user.RefreshToken == token);
        }
        public User Create(User user)
        {
            _context.Users.Add(user);
            _context.SaveChanges();
            return user;
        }

        public async void UpdateSocialId(string email, string? socialId, string provider)
        {
            var user = await GetByEmail(email);
            if (user == null)
            {
                throw new Exception("User not found");
            }

            switch (provider)
            {
                case "google":
                    user.GoogleId = socialId;
                    break;
                case "facebook":
                    user.FacebookId = socialId;
                    break;
                case "apple":
                    user.AppleId = socialId;
                    break;
                default:
                    throw new Exception("Invalid provider");
            }

           await _context.SaveChangesAsync();
        }

        public async Task UpdateRefreshToken(string email, string? encryptedRefreshToken)
        {
            User? user = await GetByEmail(email);
            if (user == null)
            {
                throw new Exception("User not found");
            }

            user.RefreshToken = encryptedRefreshToken;

            await _context.SaveChangesAsync();
        }

        public static UserTokenInfo GetTokenPayloadFromUser(User user)
        {
            return new UserTokenInfo
            {
                UserId = user.Id.ToString(),
                Email = user.Email
            };
        }

    }
}