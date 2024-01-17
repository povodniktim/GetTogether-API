namespace API.Models.Response.User
{
    public class GetUserResponse
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? ProfileImageUrl { get; set; }
        public int CreatedEventsCount { get; set; }
        public int AttendedEventsCount { get; set; }
        public int NotificationsCount { get; set; }
    }
}
