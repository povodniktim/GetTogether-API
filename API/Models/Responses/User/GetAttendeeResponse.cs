namespace API.Models.Responses.User
{
    public class GetAttendeeResponse
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string? ProfileImageUrl { get; set; }
    }
}
