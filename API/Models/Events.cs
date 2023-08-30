using System;
using System.Collections.Generic;

namespace API.Models;

public partial class Events
{
    public int ID { get; set; }

    public int organizerID { get; set; }

    public int activityID { get; set; }

    public DateTime createdAt { get; set; }

    public string title { get; set; } = null!;

    public string? description { get; set; }

    public DateTime date { get; set; }

    public string? location { get; set; }

    public int? maxParticipants { get; set; }

    public string? visibility { get; set; }

    public virtual ICollection<EventParticipants> EventParticipants { get; set; } = new List<EventParticipants>();

    public virtual ICollection<Notifications> Notifications { get; set; } = new List<Notifications>();

    public virtual Activities activity { get; set; } = null!;

    public virtual Users organizer { get; set; } = null!;
}
