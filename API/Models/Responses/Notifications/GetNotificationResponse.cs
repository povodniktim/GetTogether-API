using API.Models.Response.User;

namespace API.Models.Responses.Notifications
{
    public class GetNotificationResponse
    {
        public int UserId { get; set; }

        public int? EventId { get; set; }

        public int? ParticipantId { get; set; }

        public string? Status { get; set; }

        public GetUserResponse User { get; set; }

        public GetUserResponse Participant { get; set; }
    }
}
