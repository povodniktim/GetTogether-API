using API.Models.Response.User;
using API.Models.Responses.Event;

namespace API.Models.Responses.Notifications
{
    public class GetNotificationResponse
    {
        public int OrganizerId { get; set; }

        public int? EventId { get; set; }

        public int? ParticipantId { get; set; }

        public DateTime CreatedAt { get; set; }

        public string Status { get; set; }

        public GetUserResponse Organizer { get; set; }

        public GetUserResponse Participant { get; set; }

        public GetEventSimpleResponse Event { get; set; }
    }
}
