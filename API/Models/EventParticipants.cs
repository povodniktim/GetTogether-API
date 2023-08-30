using System;
using System.Collections.Generic;

namespace API.Models;

public partial class EventParticipants
{
    public int ID { get; set; }

    public int participantID { get; set; }

    public int eventID { get; set; }

    public string? status { get; set; }

    public DateTime? statusChangedAt { get; set; }

    public virtual ICollection<Notifications> Notifications { get; set; } = new List<Notifications>();

    public virtual Events _event { get; set; } = null!;

    public virtual Users participant { get; set; } = null!;
}
