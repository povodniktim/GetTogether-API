namespace API.Models.Responses.User
{
    public class GetParticipantResponse
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? ProfileImageUrl { get; set; }
    }
}
