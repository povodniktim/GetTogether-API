namespace API.Models.Requests.Event
{
    public class GetEventRequest
    {
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
