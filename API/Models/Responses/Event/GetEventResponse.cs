﻿using API.Models.Responses.Activity;
using API.Models.Responses.User;

namespace API.Models.Responses.Event
{
    public class GetEventResponse
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
        public List<GetAttendeeResponse> Attendees { get; set; }
        public GetOrganizerResponse Organizer { get; set; }
        public GetActivityResponse Activity { get; set; }
    }
}
