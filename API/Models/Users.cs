using System;
using System.Collections.Generic;

namespace API.Models;

public partial class Users
{
    public int ID { get; set; }

    public string firstName { get; set; } = null!;

    public string lastName { get; set; } = null!;

    public string email { get; set; } = null!;

    public DateTime createdAt { get; set; }

    public string? profileImageUrl { get; set; }

    public string? googleID { get; set; }

    public string? facebookID { get; set; }

    public string? twitterID { get; set; }

    public string? appleID { get; set; }

    public virtual ICollection<EventParticipants> EventParticipants { get; set; } = new List<EventParticipants>();

    public virtual ICollection<Events> Events { get; set; } = new List<Events>();

    public virtual ICollection<Notifications> Notifications { get; set; } = new List<Notifications>();
}
