﻿namespace API.Models;

public partial class EventParticipant
{
    public int Id { get; set; }

    public int ParticipantId { get; set; }

    public int EventId { get; set; }

    // TODO: we currently don't use this
    public string? Status { get; set; } = "going";

    public DateTime? CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    public virtual Event Event { get; set; } = null!;

    public virtual ICollection<Notification> Notifications { get; set; } = new List<Notification>();

    public virtual User Participant { get; set; } = null!;
}
