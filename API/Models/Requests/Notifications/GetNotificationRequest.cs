using API.Models.Response.User;

namespace API.Models.Requests.Notifications
{
    public class GetNotificationRequest
    {
        public int UserId { get; set; }

        public int? EventId { get; set; }

        public int? ParticipantId { get; set; }

        public string? Status { get; set; }

        public GetUserResponse User { get; set; }

        public GetUserResponse Participant { get; set; }
    }
}
