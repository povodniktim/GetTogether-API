using System;
using System.Collections.Generic;

namespace API.Models;

public partial class Users
{
    public int ID { get; set; }

    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    public string? Email { get; set; }

    public string? Password { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? LastLoggedInAt { get; set; }

    public virtual ICollection<EventComments> EventComments { get; set; } = new List<EventComments>();

    public virtual ICollection<EventParticipants> EventParticipants { get; set; } = new List<EventParticipants>();

    public virtual ICollection<Events> Events { get; set; } = new List<Events>();
}
