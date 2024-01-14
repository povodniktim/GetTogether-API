namespace API.Models.Responses.Event
{
    public class JoinEventResponse
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public int OrganizerId { get; set; }
        public int ActivityId { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Description { get; set; }
        public DateTime Date { get; set; }
        public string Location { get; set; }
        public int? MaxParticipants { get; set; }
        public string Visibility { get; set; }
    }
}
