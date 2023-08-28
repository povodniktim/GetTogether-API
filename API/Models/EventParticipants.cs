using System;
using System.Collections.Generic;

namespace API.Models;

public partial class EventParticipants
{
    public int ID { get; set; }

    public int EventID { get; set; }

    public int ParticipantID { get; set; }

    public virtual Events Event { get; set; } = null!;

    public virtual Users Participant { get; set; } = null!;
}
