using System;
using System.Collections.Generic;

namespace API.Models;

public partial class Events
{
    public int ID { get; set; }

    public int OrganizerID { get; set; }

    public DateTime CreatedAt { get; set; }

    public string Title { get; set; } = null!;

    public string Description { get; set; } = null!;

    public DateTime Date { get; set; }

    public string? Location { get; set; }

    public int MaxParticipants { get; set; }

    public virtual ICollection<EventComments> EventComments { get; set; } = new List<EventComments>();

    public virtual ICollection<EventParticipants> EventParticipants { get; set; } = new List<EventParticipants>();

    public virtual ICollection<EventPhotos> EventPhotos { get; set; } = new List<EventPhotos>();

    public virtual Users Organizer { get; set; } = null!;
}
