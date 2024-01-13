using System.ComponentModel.DataAnnotations;

namespace API.Models
{
    public class UserSocialSignIn
    {
        [Required(ErrorMessage = "Id is required")]
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        [Url(ErrorMessage = "Invalid profile image url")]
        public string? ProfileImageUrl { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        public string Email { get; set; }

    }

}