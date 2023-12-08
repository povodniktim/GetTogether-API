using System;
using System.Collections.Generic;

namespace API.Models;

public partial class EventParticipant
{
    public int Id { get; set; }

    public int ParticipantId { get; set; }

    public int EventId { get; set; }

    public string? Status { get; set; }

    public DateTime? StatusChangedAt { get; set; }

    public virtual Event Event { get; set; } = null!;

    public virtual ICollection<Notification> Notifications { get; set; } = new List<Notification>();

    public virtual User Participant { get; set; } = null!;
}
