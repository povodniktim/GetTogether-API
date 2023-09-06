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

        public User? GetByEmail(string email)
        {
            return _context.Users.FirstOrDefault(user => user.Email == email);
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

        public void UpdateRefreshToken(string email, string encryptedRefreshToken)
        {
            var user = GetByEmail(email);
            if (user == null)
            {
                throw new Exception("User not found");
            }

            user.RefreshToken = encryptedRefreshToken;
            _context.SaveChanges();
        }

        public static Dictionary<string, string> GetTokenPayloadFromUser(User user)
        {
            return new Dictionary<string, string>
            {
                { "userId", user.Id.ToString() },
                { "email", user.Email }
            };
        }

    }
}